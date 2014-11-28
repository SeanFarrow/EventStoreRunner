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

namespace EventStoreRunner
{
    public class EventStoreRunnerOptionsBuilder
    {
        private bool _useembeddedclient;

        private bool _purgedata;
        private bool _runinmemory;
        private string _datadirectory;
        internal EventStoreRunnerOptionsBuilder UseEmbeddedClient()
        {
            _useembeddedclient = true;
            _runinmemory = true;
            _datadirectory = "data";
            return this;
        }

        public EventStoreRunnerOptionsBuilder RunInMemory()
        {
            _runinmemory = true;
            return this;
        }
        
        public EventStoreRunnerOptionsBuilder UseFullServer()
        {
            _useembeddedclient = false;
            return this;
        }

        public EventStoreRunnerOptionsBuilder UseDataDirectory(string directory)
        {
            _datadirectory = directory;
            return this;
        }

        public EventStoreRunnerOptionsBuilder PurgeData()
        {
            _purgedata = true;
            return this;
        }
        
        public EventStoreRunnerOptions Build()
        {
            return new EventStoreRunnerOptions(_useembeddedclient, _runinmemory, _datadirectory, _purgedata);
        }
    }
}