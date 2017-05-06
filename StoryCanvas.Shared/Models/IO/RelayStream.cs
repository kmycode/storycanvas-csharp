using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StoryCanvas.Shared.Models.IO
{
	public class RelayStream : Stream, IDisposable
	{
		private Stream _stream;

		public override bool CanRead
		{
			get
			{
				return this._stream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this._stream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this._stream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				return this._stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this._stream.Position;
			}

			set
			{
				this._stream.Position = value;
			}
		}

		private bool _isFlushWhenWrite = false;

		public RelayStream(Stream stream, bool isFlushWhenWrite = true) { this._stream = new MemoryStream(); this._isFlushWhenWrite = isFlushWhenWrite; }

		public override void Flush()
		{
			this._stream.Flush();
			this.Flushed?.Invoke(this, new FlushedEventArgs(this));
			this.Flushed = null;
		}

		public void Finish()
		{
			this.Flushed?.Invoke(this, new FlushedEventArgs(this));
			this._stream.Dispose();
			this._stream = null;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this._stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			this._stream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this._stream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this._stream.Write(buffer, offset, count);
			if (this._isFlushWhenWrite)
			{
				this.Flushed?.Invoke(this, new FlushedEventArgs(this));
				//this.Flushed = null;
				//this._stream.Dispose();
			}
		}

		public new void Dispose()
		{
			//this._stream.Dispose();
		}

		public event FlushedEventHandler Flushed;
	}

	public delegate void FlushedEventHandler(object sender, FlushedEventArgs e);

	public class FlushedEventArgs : EventArgs
	{
		public RelayStream Stream { get; }
		public FlushedEventArgs(RelayStream stream)
		{
			this.Stream = stream;
		}
	}
}
