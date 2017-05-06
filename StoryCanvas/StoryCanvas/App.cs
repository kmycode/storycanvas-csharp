using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using StoryCanvas.Messages.IO;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Messages.Network;
using StoryCanvas.Shared.Messages.Page;
using StoryCanvas.Shared.Messages.Picker;
using StoryCanvas.Shared.Models.IO;
using StoryCanvas.Shared.Types;
using StoryCanvas.Shared.Utils;
using StoryCanvas.Shared.ViewModels;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.View.Common;
using StoryCanvas.View.CustomControls.Pickers;
using StoryCanvas.View.Editors;
using StoryCanvas.View.Pages;
using Xamarin.Forms;

namespace StoryCanvas
{
    public class App : Application, IAuthBrowserProvider
	{
		private void SetMainModeChangableEvent(Page page)
		{
			page.Disappearing += (sender, e) =>
			{
				StoryViewModel.Default.IsMainModeChangable = true;
			};
		}

		private StartPage _startPage;
		private StartPage StartPage
		{
			get
			{
				if (this._startPage == null)
				{
					this._startPage = new StartPage();
				}
				return this._startPage;
			}
		}

		private MainPage __mainPage;
		private MainPage _mainPage
		{
			get
			{
				if (this.__mainPage == null)
				{
					this.__mainPage = new MainPage();
				}
				return this.__mainPage;
			}
		}

		private EditPeoplePage _editPeoplePage;
		private EditPeoplePage EditPeoplePage
		{
			get
			{
				if (this._editPeoplePage == null)
				{
					this._editPeoplePage = new EditPeoplePage();
					this.SetMainModeChangableEvent(this._editPeoplePage);
				}
				return this._editPeoplePage;
			}
		}

		private EditGroupPage _editGroupPage;
		private EditGroupPage EditGroupPage
		{
			get
			{
				if (this._editGroupPage == null)
				{
					this._editGroupPage = new EditGroupPage();
					this.SetMainModeChangableEvent(this._editGroupPage);
				}
				return this._editGroupPage;
			}
		}

		private EditPlacePage _editPlacePage;
		private EditPlacePage EditPlacePage
		{
			get
			{
				if (this._editPlacePage == null)
				{
					this._editPlacePage = new EditPlacePage();
					this.SetMainModeChangableEvent(this._editPlacePage);
				}
				return this._editPlacePage;
			}
		}

		private EditScenePage _editScenePage;
		private EditScenePage EditScenePage
		{
			get
			{
				if (this._editScenePage == null)
				{
					this._editScenePage = new EditScenePage();
					this.SetMainModeChangableEvent(this._editScenePage);
				}
				return this._editScenePage;
			}
		}

		private EditChapterPage _editChapterPage;
		private EditChapterPage EditChapterPage
		{
			get
			{
				if (this._editChapterPage == null)
				{
					this._editChapterPage = new EditChapterPage();
					this.SetMainModeChangableEvent(this._editChapterPage);
				}
				return this._editChapterPage;
			}
		}

		private EditSexPage _editSexPage;
		private EditSexPage EditSexPage
		{
			get
			{
				if (this._editSexPage == null)
				{
					this._editSexPage = new EditSexPage();
					this.SetMainModeChangableEvent(this._editSexPage);
				}
				return this._editSexPage;
			}
		}

		private EditParameterPage _editParameterPage;
		private EditParameterPage EditParameterPage
		{
			get
			{
				if (this._editParameterPage == null)
				{
					this._editParameterPage = new EditParameterPage();
					this.SetMainModeChangableEvent(this._editParameterPage);
				}
				return this._editParameterPage;
			}
		}

		private EditMemoPage _editMemoPage;
		private EditMemoPage EditMemoPage
		{
			get
			{
				if (this._editMemoPage == null)
				{
					this._editMemoPage = new EditMemoPage();
					this.SetMainModeChangableEvent(this._editMemoPage);
				}
				return this._editMemoPage;
			}
		}

		private EditWordPage _editWordPage;
		private EditWordPage EditWordPage
		{
			get
			{
				if (this._editWordPage == null)
				{
					this._editWordPage = new EditWordPage();
					this.SetMainModeChangableEvent(this._editWordPage);
				}
				return this._editWordPage;
			}
		}

		private StorySettingPage _storySettingPage;
		private StorySettingPage StorySettingPage
		{
			get
			{
				if (this._storySettingPage == null)
				{
					this._storySettingPage = new StorySettingPage();
					this.SetMainModeChangableEvent(this._storySettingPage);
				}
				return this._storySettingPage;
			}
		}

		private ChapterTextPage _chapterTextPage;
		private ChapterTextPage ChapterTextPage
		{
			get
			{
				if (this._chapterTextPage == null)
				{
					this._chapterTextPage = new ChapterTextPage();
					this.SetMainModeChangableEvent(this._chapterTextPage);
				}
				return this._chapterTextPage;
			}
		}

		private TimelinePage _timelinePage;
		private TimelinePage TimelinePage
		{
			get
			{
				if (this._timelinePage == null)
				{
					this._timelinePage = new TimelinePage();
					this.SetMainModeChangableEvent(this._timelinePage);
				}
				return this._timelinePage;
			}
		}

		private AboutPage _aboutPage;
		private AboutPage AboutPage
		{
			get
			{
				if (this._aboutPage == null)
				{
					this._aboutPage = new AboutPage();
					this.SetMainModeChangableEvent(this._aboutPage);
				}
				return this._aboutPage;
			}
		}

		private SlotManagementPage _slotPage;
		private SlotManagementPage SlotPage
		{
			get
			{
				if (this._slotPage == null)
				{
					this._slotPage = new SlotManagementPage();
					this.SetMainModeChangableEvent(this._slotPage);
				}
				return this._slotPage;
			}
		}

		private StoragePage _storagePage;
		private StoragePage StoragePage
		{
			get
			{
				if (this._storagePage == null)
				{
					this._storagePage = new StoragePage();
					this.SetMainModeChangableEvent(this._storagePage);
				}
				return this._storagePage;
			}
		}

		private SceneDesignerPage _sceneDesignerPage;
		private SceneDesignerPage SceneDesignerPage
		{
			get
			{
				if (this._sceneDesignerPage == null)
				{
					this._sceneDesignerPage = new SceneDesignerPage();
					this.SetMainModeChangableEvent(this._sceneDesignerPage);
				}
				return this._sceneDesignerPage;
			}
		}

		private SlotPickerMessage SlotPickerMessage;

		private bool _isStartPage = true;						// ���݃X�^�[�g�y�[�W�ɂ��邩

        public App()
        {
			// �A�v���P�[�V�����S�̂̏�����
			StoryCanvasApplication.Initialize();

			this.MainPage = new NavigationPage(StartPage);
			this._isStartPage = true;

			// ��ʃ��[�h���ς�������Ƀy�[�W�J�ځi��ʃ��[�h�I����ʂƕҏW��ʂ𓯎��ɕ\���ł��Ȃ��̂�WPF�ƈ���ĕ��ʂ�Binding�ł��Ȃ��j
			StoryViewModel.Default.PropertyChanged += async (sender, e) =>
			{
				if (e.PropertyName == "MainMode")
				{
					var mainMode = StoryViewModel.Default.MainMode;

					// �X�^�[�g�y�[�W���\������Ă���Ƃ��͕���
					// �iWPF�ƈقȂ�A�X�^�[�g�y�[�W�̃��j���[�ȊO�ɃN���b�N�ł���Ƃ��낪�Ȃ��j
					if (_isStartPage)
					{
						await this.MainPage.Navigation.PushAsync(this._mainPage);
						this.MainPage.Navigation.RemovePage(this.StartPage);
						this._isStartPage = false;
						StoryViewModel.Default.IsMainModeChangable = true;
					}

					switch (mainMode)
					{
						case MainMode.EditPerson:
							await this.MainPage.Navigation.PushAsync(this.EditPeoplePage);
							break;
						case MainMode.EditGroup:
							await this.MainPage.Navigation.PushAsync(this.EditGroupPage);
							break;
						case MainMode.EditPlace:
							await this.MainPage.Navigation.PushAsync(this.EditPlacePage);
							break;
						case MainMode.EditScene:
							await this.MainPage.Navigation.PushAsync(this.EditScenePage);
							break;
						case MainMode.EditChapter:
							await this.MainPage.Navigation.PushAsync(this.EditChapterPage);
							break;
						case MainMode.EditWord:
							await this.MainPage.Navigation.PushAsync(this.EditWordPage);
							break;

						case MainMode.EditSex:
							await this.MainPage.Navigation.PushAsync(this.EditSexPage);
							break;
						case MainMode.EditParameter:
							await this.MainPage.Navigation.PushAsync(this.EditParameterPage);
							break;
						case MainMode.EditMemo:
							await this.MainPage.Navigation.PushAsync(this.EditMemoPage);
							break;

						case MainMode.StorySettingPage:
							await this.MainPage.Navigation.PushAsync(this.StorySettingPage);
							break;
						case MainMode.ChapterTextPage:
							await this.MainPage.Navigation.PushAsync(this.ChapterTextPage);
							break;
						case MainMode.SceneDesignerPage:
							await this.MainPage.Navigation.PushAsync(this.SceneDesignerPage);
							break;

						case MainMode.TimelinePage:
							await this.MainPage.Navigation.PushAsync(this.TimelinePage);
							break;

						case MainMode.NetworkPage:
							{
								var page = new NetworkPage();
								this.SetMainModeChangableEvent(page);
								await this.MainPage.Navigation.PushAsync(page);
							}
							break;
						case MainMode.AboutPage:
							await this.MainPage.Navigation.PushAsync(this.AboutPage);
							break;
						case MainMode.SlotPage:
							await this.MainPage.Navigation.PushAsync(this.SlotPage);
							break;
						case MainMode.StoragePage:
							await this.MainPage.Navigation.PushAsync(this.StoragePage);
							break;
					}
				}
			};

			// UI�X���b�h�ŏ������s��
			Messenger.Default.Register<UIThreadInvokeMessage>((message) =>
			{
				Device.BeginInvokeOnMainThread(message.Action);
			});

			// ���t�I����ʂ�\��
			Messenger.Default.Register<DatePickerMessage>(async (message) =>
			{
				await this.MainPage.Navigation.PushAsync(new StoryDatePicker(message), true);
			});

			// ���t�����I����ʂ�\��
			Messenger.Default.Register<DateTimePickerMessage>(async (message) =>
			{
				await this.MainPage.Navigation.PushAsync(new StoryDateTimePicker(message), true);
			});

			// �F�I����ʂ�\��
			Messenger.Default.Register<ColorPickerMessage>(async (message) =>
			{
				await this.MainPage.Navigation.PushAsync(new ColorPicker(message), true);
			});

			// �G���e�B�e�B�̊֌W��ҏW����ڍ׉�ʂ�\��
			Messenger.Default.Register<EditEntityRelationMessage>(async (message) =>
			{
				await this.MainPage.Navigation.PushAsync(new EntityRelationEditor { BindingContext = message.Relation });
			});

			// �֘A�t����ꂽ�G���e�B�e�B�̈ꗗ��ҏW�����ʂ�\��
			Messenger.Default.Register<EntityRelationEditorMessage>(async (message) =>
			{
				await this.MainPage.Navigation.PushAsync(new EntityRelationPage(message));
			});

			// �e�L�X�g��ҏW�����ʂ�\��
			Messenger.Default.Register<EditTextMessage>(async (message) =>
			{
				await this.MainPage.Navigation.PushAsync(new TextEditorPage(message));
			});

			// �X���b�g�I����ʂ�\��
			Messenger.Default.Register<SaveSlotPickerMessage>((message) =>
			{
				this.SlotPickerMessage = message;
				this.MainPage.Navigation.PushAsync(new SlotChooserPage());
			});

			// �X���b�g�I����ʁi�t�@�C���J���̂݁j��\��
			Messenger.Default.Register<OpenSlotPickerMessage>((message) =>
			{
				this.SlotPickerMessage = message;
				this.MainPage.Navigation.PushAsync(new SlotChooserPage(true));
			});

			// �X���b�g�I����ʂŃX���b�g��I��������
			Messenger.Default.Register<SlotChoosedMessage>((message) =>
			{
				if (this.SlotPickerMessage != null)
				{
					this.SlotPickerMessage.OnPicked(message.File);
					this.SlotPickerMessage = null;
					//Messenger.Default.Send(this, new NavigationBackMessage());
				}
			});

			// �l�b�g���[�N�̃y�[�W���J��
			Messenger.Default.Register<OpenNetworkPageMessage>(async (message) =>
			{
				await this.MainPage.Navigation.PushAsync(new NetworkPage());
			});

			// �l�b�g���[�N�֑��M���邽�߂̃y�[�W���J��
			Messenger.Default.Register<OpenNetworkSendMessage>(async (message) =>
			{
				await this.MainPage.Navigation.PushAsync(new NetworkSendPage());
			});

			// �l�b�g���[�N�����M���邽�߂̃y�[�W���J��
			Messenger.Default.Register<OpenNetworkReceiveMessage>(async (message) =>
			{
				// �X�^�[�g�y�[�W���\������Ă���Ƃ��͕���
				// �i�X�^�[�g�y�[�W���璼�s����\�������邽�߁j
				if (_isStartPage)
				{
					await this.MainPage.Navigation.PushAsync(this._mainPage);
					this.MainPage.Navigation.RemovePage(this.StartPage);
					this._isStartPage = false;
				}

				await this.MainPage.Navigation.PushAsync(new NetworkReceivePage());
			});

			// �A���[�g��\��
			Messenger.Default.Register<AlertMessage>(async (message) =>
			{
				AlertMessageResult result = default(AlertMessageResult);
				switch (message.Type)
				{
					case AlertMessageType.Ok:
						await this.MainPage.DisplayAlert(AppResources.ApplicationName, message.Text, "OK");
						result = AlertMessageResult.Ok;
						break;
					case AlertMessageType.OkCancel:
						result = await this.MainPage.DisplayAlert(AppResources.ApplicationName, message.Text, "OK", StringResourceResolver.Resolve("Cancel")) ? AlertMessageResult.Ok : AlertMessageResult.Cancel;
						break;
					case AlertMessageType.Yes:
						await this.MainPage.DisplayAlert(AppResources.ApplicationName, message.Text, StringResourceResolver.Resolve("Yes"));
						break;
					case AlertMessageType.YesNo:
						result = await this.MainPage.DisplayAlert(AppResources.ApplicationName, message.Text, StringResourceResolver.Resolve("Yes"), StringResourceResolver.Resolve("No")) ? AlertMessageResult.Yes : AlertMessageResult.No;
						break;
					case AlertMessageType.YesNoCancel:
						string r = await this.MainPage.DisplayActionSheet(message.Text, StringResourceResolver.Resolve("Cancel"), null, StringResourceResolver.Resolve("Yes"), StringResourceResolver.Resolve("No"));
						result = r == StringResourceResolver.Resolve("Yes") ? AlertMessageResult.Yes :
									r == StringResourceResolver.Resolve("No") ? AlertMessageResult.No : AlertMessageResult.Cancel;
						break;
				}
				message.Result = result;
				if (message.Continue != null)
				{
					message.Continue(message.Result);
				}
			});

			// �A���[�g�܂��̓g�[�X�g��\��
			Messenger.Default.Register<LightAlertMessage>((message) =>
			{
				if (!LightAlert.Alert(message.Text))
				{
					this.MainPage.DisplayAlert(AppResources.ApplicationName, message.Text, "OK");
				}
			});

			// �V�[���̃y�[�W��\������
			Messenger.Default.Register<StartEditSceneMessage>(async (message) =>
			{
				await this.MainPage.Navigation.PushAsync(new TemporarySceneEditorPage(message));
				message.ContinueWhenEditStarted();
			});

			// �l���̃y�[�W��\������
			Messenger.Default.Register<StartEditPersonMessage>(async (message) =>
			{
				await this.MainPage.Navigation.PushAsync(new TemporaryPersonEditorPage(message));
				message.ContinueWhenEditStarted();
			});

			// �ꏊ�̃y�[�W��\������
			Messenger.Default.Register<StartEditPlaceMessage>(async (message) =>
			{
				await this.MainPage.Navigation.PushAsync(new TemporaryPlaceEditorPage(message));
				message.ContinueWhenEditStarted();
			});

			// �P�O�̃y�[�W�ɖ߂�
			Messenger.Default.Register<NavigationBackMessage>(async (message) =>
			{
				await this.MainPage.Navigation.PopAsync(true);
			});

			// ���C���y�[�W�ɖ߂�
			Messenger.Default.Register<NavigationBackToRootMessage>(async (message) =>
			{
				await this.MainPage.Navigation.PopToRootAsync(true);
			});

			// �w���URL���J��
			Messenger.Default.Register<OpenUrlMessage>((message) =>
			{
				Device.OpenUri(new Uri(message.Url));
			});

			// Autofac�Ɏ��g��o�^
			AutofacUtil.AuthBrowserProvider = this;

			// �p�����[�^�ҏW��ʂ�static�R���X�g���N�^�����s������
			new EditParameterPage();
		}

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
			// Handle when your app sleeps
			Shared.Models.Story.StoryModel.Current.Quit();
			System.Diagnostics.Debug.WriteLine("Hello");
		}

        protected override void OnResume()
        {
			// Handle when your app resumes
			StorageModelBase.LastUseStorage?.CheckSaveConflict();
        }

		public IAuthBrowser OpenBrowser()
		{
			var page = new AuthBrowserPage();
			this.MainPage.Navigation.PushAsync(page);
			return page;
		}

		/// <summary>
		/// �ꎞ�I�ɗ��p�����V�[���ҏW���
		/// </summary>
		private class TemporarySceneEditorPage : ContentPage
		{
			private StartEditSceneMessage Message;

			public TemporarySceneEditorPage(StartEditSceneMessage message)
			{
				this.Content = new SceneEditor();
				this.Title = AppResources.Scene;
				message.ContinueWhenEditStarted();
				this.Message = message;
			}

			protected override void OnDisappearing()
			{
				base.OnDisappearing();
				this.Message.OnEditorClosed();
			}
		}

		/// <summary>
		/// �ꎞ�I�ɗ��p�����l���ҏW���
		/// </summary>
		private class TemporaryPersonEditorPage : PersonEditorPage
		{
			private StartEditPersonMessage Message;

			public TemporaryPersonEditorPage(StartEditPersonMessage message)
			{
				message.ContinueWhenEditStarted();
				this.Title = AppResources.Person;
				this.Message = message;
			}

			protected override void OnDisappearing()
			{
				base.OnDisappearing();
				this.Message.OnEditorClosed();
			}
		}

		/// <summary>
		/// �ꎞ�I�ɗ��p�����ꏊ�ҏW���
		/// </summary>
		private class TemporaryPlaceEditorPage : ContentPage
		{
			private StartEditPlaceMessage Message;

			public TemporaryPlaceEditorPage(StartEditPlaceMessage message)
			{
				this.Content = new PlaceEditor();
				this.Title = AppResources.Place;
				message.ContinueWhenEditStarted();
				this.Message = message;
			}

			protected override void OnDisappearing()
			{
				base.OnDisappearing();
				this.Message.OnEditorClosed();
			}
		}
	}
}
