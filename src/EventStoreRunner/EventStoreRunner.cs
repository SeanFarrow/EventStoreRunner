// Copyright 2014 Sean Farrow
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using EventStore.ClientAPI.Embedded;
using EventStore.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace EventStoreRunner
{
    public class EventStoreRunner: IDisposable
    {
        private ClusterVNode _embeddednode; //The embedded server.
        
        private bool _disposed;

        private EventStoreRunnerOptions _runneroptions;

        private Process _fullserverprocess;
        
        public EventStoreRunner(EventStoreRunnerOptions options)
        {
            _runneroptions = options;
            if (_runneroptions.UseEmbeddedClient)
            {
                CreateEmbeddedServer();
            }
            else
            {
                CreateFullServer();
            }
        }

        private void CreateFullServer()
        {
            var thread = new Thread(StartEventStoreServer)
            {
                IsBackground = true
            };
            thread.Start();
        }

        private void StartEventStoreServer()
        {
            var eventStorePath = Path.Combine(Assembly.GetExecutingAssembly().GetExecutingFolder(), "EventStoreBinaries", "EventStore.ClusterNode.exe");
            if (File.Exists(eventStorePath))
            {
                throw new FileNotFoundException("The EventStore binaries are not in the project directory structure.");
            }
            var startInfo = new ProcessStartInfo
            {
                FileName = eventStorePath,
                WindowStyle = ProcessWindowStyle.Normal,
                ErrorDialog = true,
                LoadUserProfile = true,
                CreateNoWindow = false,
                UseShellExecute = false
            };
            
            string cmdline = null;
            if (_runneroptions.DataDirectory.Any())
            {
                string combinedPath;
                if (Path.IsPathRooted(_runneroptions.DataDirectory))
                {
                    combinedPath = _runneroptions.DataDirectory;
                }
                else
                {
                    combinedPath = Path.Combine(Assembly.GetExecutingAssembly().GetExecutingFolder(), _runneroptions.DataDirectory);
                }
                cmdline = String.Format("--db={0}", combinedPath);
            }
            if (!String.IsNullOrEmpty(cmdline))
            {
                startInfo.Arguments = cmdline;
            }
            
            try
            {
                this._fullserverprocess = new Process { StartInfo = startInfo };

                this._fullserverprocess.Start();
                this._fullserverprocess.WaitForExit();
            }
            catch
            {
                this._fullserverprocess.CloseMainWindow();
                this._fullserverprocess.Dispose();
            }
        }

        private void CreateEmbeddedServer()
        {
            EmbeddedVNodeBuilder builder;
            builder = EmbeddedVNodeBuilder.AsSingleNode();
            builder.RunProjections(ProjectionsMode.All);
            builder.OnDefaultEndpoints();
            if (_runneroptions.RunInMemory)
            {
                builder.RunInMemory();
            }
            else
            {
                string combinedPath;
                if (Path.IsPathRooted(_runneroptions.DataDirectory))
                {
                    combinedPath = _runneroptions.DataDirectory;
                }
                else
                {
                    combinedPath = Path.Combine(Assembly.GetExecutingAssembly().GetExecutingFolder(), _runneroptions.DataDirectory);
                }
                builder.RunOnDisk(combinedPath);
            }
            _embeddednode = builder.Build();
            _embeddednode.Start();
        }
            
        public void Dispose()
        {
            Dispose(true);
        GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                if (disposing)
                {
                    if (_runneroptions.UseEmbeddedClient)
                    {
                        DisposeEmbeddedServer();
                    }
                    else
                    {
                        DisposeFullServer();
                    }
                    PurgeDataFolder();            
                    _disposed = true;
                }
            }
        }

        private void PurgeDataFolder()
        {
            if (_runneroptions.PurgeData)
            {
                string combinedPath;
                if (Path.IsPathRooted(_runneroptions.DataDirectory))
                {
                    combinedPath = _runneroptions.DataDirectory;
                }
                else
                {
                    combinedPath = Path.Combine(Assembly.GetExecutingAssembly().GetExecutingFolder(), _runneroptions.DataDirectory);
                }
Directory.Delete(combinedPath);
            }
        }
        
        private void DisposeFullServer()
        {
            _fullserverprocess.Kill();
            _fullserverprocess.Dispose();
            }

        private void DisposeEmbeddedServer()
        {
            _embeddednode.Stop();
            _embeddednode = null;
        }
    }
}