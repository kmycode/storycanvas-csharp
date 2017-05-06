using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StoryCanvas.Shared.Messages.IO;
using System.Drawing.Printing;

namespace StoryCanvas.WPF.Models
{
	/// <summary>
	/// テキストページを印刷するモデル
	/// </summary>
	class PrintTextPageModel
	{
		private PrintTextPageMessage Message;
		private int CurrentTextesIndex = 0;

		public PrintTextPageModel(PrintTextPageMessage message)
		{
			// http://dobon.net/vb/dotnet/graphics/printtext.html
			this.Message = message;
		}
		
		private int CharacterCount;		// 現在印字中の文字列で、何文字目を印刷中であるか
		private int? printY = null;

		private Font textFont;
		private Font titleFont;
		private Font subTitleFont;
		private bool isTitlePrinted = false;
		private bool isSubTitlePrinted = false;

		//Button1のClickイベントハンドラ
		public void Print()
		{
			//印刷する文字列と位置を設定する
			CharacterCount = 0;

			//印刷に使うフォントを指定する
			this.textFont = new Font("ＭＳ Ｐゴシック", (int)this.Message.PageInfo.FontSize);
			this.titleFont = new Font("ＭＳ Ｐゴシック", (int)this.Message.PageInfo.TitleFontSize);
			this.subTitleFont = new Font("ＭＳ Ｐゴシック", (int)this.Message.PageInfo.SubTitleFontSize);

			//PrintDocumentオブジェクトの作成
			System.Drawing.Printing.PrintDocument pd =
				new System.Drawing.Printing.PrintDocument();

			//PrintPageイベントハンドラの追加
			pd.PrintPage +=
				new System.Drawing.Printing.PrintPageEventHandler(PrintMain);

			//印刷ダイアログ表示
			System.Windows.Forms.PrintDialog pdlg = new System.Windows.Forms.PrintDialog();
			pdlg.Document = pd;
			if (pdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				//OKがクリックされた時は印刷する
				pd.Print();
			}
		}

		private void PrintMain(object sender, PrintPageEventArgs e)
		{
			// １ページごとに呼ばれるので注意　どこまで処理したかはフィールド変数に保存する

			// ページ全体のタイトル
			if (!this.isTitlePrinted && this.Message.PageInfo.IsTitle)
			{
				this.PrintText(e, this.titleFont, this.Message.Title, false);
				this.printY += (int)(this.Message.PageInfo.TitleFontSize + this.Message.PageInfo.TitleMarginBottom);
				this.isTitlePrinted = true;
			}

			// テキスト
			while (this.CurrentTextesIndex < this.Message.Textes.Count)
			{
				// サブタイトル
				if (this.Message.PageInfo.IsSubTitle && !this.isSubTitlePrinted && this.Message.SubTitles != null && this.CurrentTextesIndex < this.Message.SubTitles.Count)
				{
					this.PrintText(e, this.subTitleFont, this.Message.SubTitles.ElementAt(this.CurrentTextesIndex), false);
					if (e.HasMorePages)
					{
						break;						// サブタイトル描画中に改ページ
					}
					else
					{
						this.CharacterCount = 0;
						this.printY += (int)this.Message.PageInfo.SubTitleMarginBottom;
						this.isSubTitlePrinted = true;
					}
				}

				// 本文
				this.PrintText(e, this.textFont, this.Message.Textes.ElementAt(this.CurrentTextesIndex), true);
				if (e.HasMorePages)
				{
					break;							// 文字列の途中で改ページ
				}
				else
				{
					this.CurrentTextesIndex++;
					this.CharacterCount = 0;        // 現在印字中の文字列内で、何文字目を印字中であるかのカウントをリセット
					this.printY += (int)this.Message.PageInfo.TextMarginBottom;      // テキストの塊同士のマージン
					this.isSubTitlePrinted = false;
				}
			}
		}

		private void PrintText(PrintPageEventArgs e, Font font, string text, bool isLineHeight)
		{
			if (CharacterCount == 0)
			{
				//改行記号を'\n'に統一する
				text = text.Replace("\r\n", "\n");
				text = text.Replace("\r", "\n");
			}

			// 文字列の最後の改行とかを取り除く
			if (this.Message.PageInfo.IsRemoveLastNewline)
			{
				text.TrimEnd('\r', '\n', '\t', ' ');
			}

			//印刷する初期位置を決定
			int startX = e.MarginBounds.Left;
			if (this.printY == null)
			{
				this.printY = e.MarginBounds.Top;
			}

			//1ページに収まらなくなるか、印刷する文字がなくなるかまでループ
			while (e.MarginBounds.Bottom > printY + font.Height &&
				CharacterCount < text.Length)
			{
				string line = "";
				for (;;)
				{
					//印刷する文字がなくなるか、
					//改行の時はループから抜けて印刷する
					if (CharacterCount >= text.Length ||
						text[CharacterCount] == '\n')
					{
						CharacterCount++;
						break;
					}
					//一文字追加し、印刷幅を超えるか調べる
					line += text[CharacterCount];
					if (e.Graphics.MeasureString(line, font).Width
						> e.MarginBounds.Width)
					{
						//印刷幅を超えたため、折り返す
						line = line.Substring(0, line.Length - 1);
						break;
					}
					//印刷文字位置を次へ
					CharacterCount++;
				}
				//一行書き出す
				e.Graphics.DrawString(line, font, Brushes.Black, startX, this.printY.Value);
				//次の行の印刷位置を計算
				printY += (int)(font.GetHeight(e.Graphics) * (isLineHeight ? this.Message.PageInfo.LineHeight : 1));
			}

			//次のページがあるか調べる
			if (CharacterCount >= text.Length)
			{
				e.HasMorePages = false;
				//初期化する
				CharacterCount = 0;
			}
			else
			{
				e.HasMorePages = true;
				this.printY = null;
			}
		}
	}
}
