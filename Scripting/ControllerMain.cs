using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Scripting.Execution;

namespace Scripting
{
	class ControllerMain
	{
		const string _fileDialogFilter = "LUA script (*.lua)|*.lua";

		public CmdWrapper CmdNewFile { get; private set; }
		public CmdWrapper CmdSaveFile { get; private set; }
		public CmdWrapper CmdLoadFile { get; private set; }
		public CmdWrapper CmdSaveAsFile { get; private set; }

		public CmdWrapper CmdPlay { get; private set; }
		public CmdWrapper CmdPause { get; private set; }
		public CmdWrapper CmdStop { get; private set; }

		private Editor.Document _document = new Editor.Document();

		private IMainWindow _presentationLevel;

		private AppExecutor _ctx;

		private void UpdateButtons()
		{
			CmdNewFile.RiseCanExecuteChanged();
			CmdSaveFile.RiseCanExecuteChanged();
			CmdLoadFile.RiseCanExecuteChanged();
			CmdSaveAsFile.RiseCanExecuteChanged();
			
			CmdPlay.RiseCanExecuteChanged();
			CmdPause.RiseCanExecuteChanged();
			CmdStop.RiseCanExecuteChanged();
		}

		public ControllerMain(IMainWindow presentationLevel, BuildInMod.BasicModule basicModule)
		{
			_presentationLevel = presentationLevel;
			_ctx = new AppExecutor(new ScriptingSDK.ModuleBase[] { basicModule });
			_ctx.OnStateChanged += new EventHandler<AppStateChangeEventArgs>(_ctx_OnStateChanged);
			_ctx.OnExecError += new EventHandler<RuntimeErrorEventArg>(_ctx_OnExecError);

			CmdNewFile = new CmdWrapper(NewFile, (object param) =>
			{
				return _ctx.ApplicationState == AppState.Idle;
			});

			CmdLoadFile = new CmdWrapper(LoadFile, (object param) =>
			{
				return _ctx.ApplicationState == AppState.Idle;
			});

			CmdSaveFile = new CmdWrapper(SaveFile, (object param) =>
			{
				return _document.IsChanged &&
					(_ctx.ApplicationState == AppState.Idle);
			});

			CmdSaveAsFile = new CmdWrapper(SaveFileAs, (object param) =>
			{
				return _ctx.ApplicationState == AppState.Idle;
			});

			CmdPlay = new CmdWrapper(Play, (object param) =>
			{
				switch (_ctx.ApplicationState)
				{
					case AppState.Idle:
					case AppState.Pause:
						return true;
					default:
						return false;
				}
			});

			CmdPause = new CmdWrapper(Pause, (object param) =>
			{
				return _ctx.ApplicationState == AppState.Running;
			});

			CmdStop = new CmdWrapper(Stop, (object param) =>
			{
				switch (_ctx.ApplicationState)
				{
					case AppState.Running:
					case AppState.Pause:
						return true;
					default:
						return false;
				}
			});
		}

		public HelpGenerator CreateHelp()
		{
			return new HelpGenerator(_ctx.Plugins);
		}

		private void NewFile(object param)
		{
			_document.NewFile();
			_presentationLevel.SetEditorContent(string.Empty);
            UpdateButtons();
        }

		private void SaveFile(object param)
		{
			if (_document.IsPathAssigned)
			{
				string content = _presentationLevel.GetEditorContent();
				_document.Save(content);
			}
			else
			{
				SaveFileAs(param);
			}

            UpdateButtons();
		}

		private void LoadFile(object param)
		{
			OpenFileDialog open = new OpenFileDialog();
			open.Filter = _fileDialogFilter;
			if (open.ShowDialog() == true)
			{
				string content = _document.Load(open.FileName);
				_presentationLevel.SetEditorContent(content);
			}

            UpdateButtons();
		}

		private void SaveFileAs(object param)
		{
			SaveFileDialog save = new SaveFileDialog();
			save.Filter = _fileDialogFilter;
			if (save.ShowDialog() == true)
			{
				string content = _presentationLevel.GetEditorContent();
				_document.SaveAs(save.FileName, content);
			}

            UpdateButtons();
		}

		private void Play(object param)
		{
			if ((_ctx != null) && (_ctx.ApplicationState == AppState.Pause))
			{
				_ctx.Resume();
			}
			else
			{
				_ctx.Run(_presentationLevel.GetEditorContent());
			}
		}

		private void Pause(object param)
		{
			_ctx.Pause();
		}

		private void Stop(object param)
		{
			_ctx.Stop();
		}

		void _ctx_OnExecError(object sender, RuntimeErrorEventArg e)
		{
			_presentationLevel.InvokeProc(() =>
			{
                if (e.RuntimeException is System.Threading.ThreadAbortException)
                {
                }
                else
                {
                    _presentationLevel.AddLogString(e.RuntimeException.Message);
                }
			});
		}

		void _ctx_OnStateChanged(object sender, AppStateChangeEventArgs e)
		{
			_presentationLevel.InvokeProc(() =>
			{
				switch (e.ApplicationState)
				{
					case AppState.Idle:
						_presentationLevel.HighLightCodeLine(0, false);
						break;
					case AppState.PausePending:
						break;
					case AppState.Pause:
						_presentationLevel.HighLightCodeLine(e.LastLine, true);
						break;
					case AppState.Running:
						_presentationLevel.HighLightCodeLine(0, false);
						break;
				}

				UpdateButtons();
			});
		}

        public void TextChanged()
        {
            _document.DocumentChanged();
            UpdateButtons();
        }
	}
}
