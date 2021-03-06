﻿// Copyright 2014 Sean Farrow
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
    public class EventStoreRunnerOptions
    {
        public EventStoreRunnerOptions(bool useEmbeddedClient, bool runInMemory, string dataDirectory, bool purgeData)
        {
            UseEmbeddedClient = useEmbeddedClient;
            RunInMemory = runInMemory;
            DataDirectory = dataDirectory;
            PurgeData = PurgeData;
        }
    public bool PurgeData { get; private set; }
        public bool UseEmbeddedClient { get; private set; }

        public bool RunInMemory { get; private set; }

        public string DataDirectory { get; private set; }

        public static EventStoreRunnerOptionsBuilder Create()
        {
            return new EventStoreRunnerOptionsBuilder();
        }
    }
}
