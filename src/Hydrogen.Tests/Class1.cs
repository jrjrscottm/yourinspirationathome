using System;
using Hydrogen.Services.Events;
using Xunit;
using Microsoft.Extensions.DependencyModel;
using System.Linq;
using System.Reflection;

namespace Hydrogen.Tests.Events
{
    public class EventBusTest
    {
        public class TestHandler : IEventHandler<string, string>
        {
            public string Handle(string message)
            {
                throw new NotImplementedException();
            }
        }

        public class Test2Handler : IEventHandler<string, string>
        {
            public string Handle(string message)
            {
                throw new NotImplementedException();
            }
        }
        [Fact]
        public void Subscribes_Message_Handlers()
        {
            var bus = new EventBus(
                new DefaultContainer(), 
                new AssemblyScannerBuilder(GetType().GetTypeInfo().Assembly));

            bus.Start();
            Assert.True(bus.Subscriptions.ContainsKey(typeof(string))); 
        }
    }
}