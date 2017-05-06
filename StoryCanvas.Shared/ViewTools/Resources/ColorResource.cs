using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using StoryCanvas.Shared.Messages.Picker;

namespace StoryCanvas.Shared.ViewTools.Resources
{
	[DataContract]
	public class ColorResource
	{
		public ColorResource(double r, double g, double b)
		{
			this.RedValue = r;
			this.GreenValue = g;
			this.BlueValue = b;
		}
		public ColorResource() { }

		public static ColorResource Default { get; private set; } = new ColorResource(0.5, 0.5, 0.5);

		[DataMember]
		public double RedValue { get; private set; }

		public byte R
		{
			get
			{
				return (byte)(this.RedValue * 255);
			}
			set
			{
				this.RedValue = (double)value / 255;
			}
		}

		[DataMember]
		public double GreenValue { get; private set; }

		public byte G
		{
			get
			{
				return (byte)(this.GreenValue * 255);
			}
			set
			{
				this.GreenValue = (double)value / 255;
			}
		}

		[DataMember]
		public double BlueValue { get; private set; }

		public byte B
		{
			get
			{
				return (byte)(this.BlueValue * 255);
			}
			set
			{
				this.BlueValue = (double)value / 255;
			}
		}

		public static readonly ColorResource Black = new ColorResource(0, 0, 0);
		public static readonly ColorResource Red = new ColorResource(1, 0, 0);
		public static readonly ColorResource Blue = new ColorResource(0, 0, 1);

		public override bool Equals(object obj)
		{
			ColorResource c = obj as ColorResource;
			if (c == null) return false;
			return this.R == c.R && this.G == c.G && this.B == c.B;
		}
	}

	[DataContract]
	public class ColorResourceWrapper : INotifyPropertyChanged
	{
		[DataMember]
		private ColorResource _color;
		public ColorResource Color
		{
			get
			{
				return this._color;
			}
			set
			{
				this._color = value;
				this.OnPropertyChanged();
			}
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
		{
			if (PropertyChanged == null) return;

			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion
	}
}
