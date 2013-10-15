using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;

namespace Scripting.Execution
{
	class AppExecutor
	{
		const string _pluginsDirectory = "plugins";

		private BackgroundWorker _worker;
		private volatile bool _isPausePending;

		public event EventHandler<RuntimeErrorEventArg> OnExecError;
		public event EventHandler<AppStateChangeEventArgs> OnStateChanged;

		public AppState ApplicationState { get; private set; }

		public Type[] Plugins { get; private set; }

		private ScriptingSDK.ModuleBase[] _buildInModules;

		private Thread _workerThread;

		public void RefreshPlugins()
		{
			const string filePrefix = @"file:\";

			string path = System.IO.Path.GetDirectoryName(typeof(AppExecutor).Assembly.CodeBase);
			path = path.Replace(filePrefix, string.Empty);
			RefreshPlugins(System.IO.Path.Combine(path, _pluginsDirectory));
		}

		public void RefreshPlugins(string path)
		{
			Plugins = ModuleLoader.LoadAllModules(path);
		}

		public AppExecutor(ScriptingSDK.ModuleBase[] buildIn)
		{
            _buildInModules = buildIn;
			RefreshPlugins();
			ApplicationState = AppState.Idle;

			_worker = new BackgroundWorker();
			_worker.WorkerReportsProgress = true;
			_worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
			_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
			_worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
			_worker.WorkerSupportsCancellation = true;
		}

		private void ChangeState(AppState state, int lineNumber)
		{
			ApplicationState = state;

			if (OnStateChanged != null)
			{
				OnStateChanged(this, new AppStateChangeEventArgs(ApplicationState, lineNumber));
			}
		}

		public void Pause()
		{
			ChangeState(AppState.PausePending, 0);
			_isPausePending = true;
		}

		public void Run(string program)
		{
			_isPausePending = false;
			_worker.RunWorkerAsync(program);
			ChangeState(AppState.Running, 0);
		}

		public void Resume()
		{
			ChangeState(AppState.Running, 0);
			_isPausePending = false;
		}

		public void Stop()
		{
			try
			{
				_workerThread.Abort();
			}
			catch
			{
			}
		}

		void _worker_DoWork(object sender, DoWorkEventArgs e)
		{
			_workerThread = Thread.CurrentThread;

			using (LuaExtender lua = new LuaExtender(Plugins))
			{
				try
				{
					lua.DebugHook += lua_DebugHook;
					lua.HookException += lua_HookException;
					lua.SetDebugHook(LuaInterface.EventMasks.LUA_MASKLINE, 0);

					foreach (ScriptingSDK.ModuleBase mod in _buildInModules)
					{
						lua.AddModule(mod);
					}

					string program = e.Argument as string;

					lua.DoString(program);

				}
				catch (ThreadAbortException)
				{
					// user cancel
					Thread.ResetAbort();
				}
				catch (Exception exc)
				{
					_worker.ReportProgress(0, exc);
				}
				finally
				{
					lua.HookException -= lua_HookException;
					lua.RemoveDebugHook();
				}
			}
		}

		void lua_HookException(object sender, LuaInterface.HookExceptionEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(e.Exception.Message);
		}

		void lua_DebugHook(object sender, LuaInterface.DebugHookEventArgs e)
		{
			if (e.LuaDebug.eventCode == LuaInterface.EventCodes.LUA_HOOKLINE)
			{
				for (bool isEntering = true; _isPausePending; isEntering = false)
				{
					if (isEntering)
					{
						ChangeState(AppState.Pause, e.LuaDebug.currentline);
					}

					System.Threading.Thread.Sleep(0);
				}
			}
		}

		void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (e.UserState is Exception)
			{
				if (OnExecError != null)
				{
					OnExecError(this, new RuntimeErrorEventArg(e.UserState as Exception));
				}
			}
		}

		void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			ChangeState(AppState.Idle, 0);
		}
	}
}
