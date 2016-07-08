using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Hydrogen.Services.Events
{
    public interface IEventHandler<T, TK>
    {
        TK Handle(T message);
    }

    public interface IEntity
    {
        List<object> Events { get; }
    }

    public interface IEventBus
    {
        
        void Publish<T>(T message);
        void Dispatch<T>(T entity) where T : IEntity;
        
    }

    public interface IStartableBus : IEventBus
    {
        void Start();
    }

    public interface IContainer
    {
        T GetService<T>();
        object GetService(Type type);
    }

    public class DefaultContainer : IContainer
    {
        public object GetService(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public T GetService<T>()
        {
            return Activator.CreateInstance<T>();
        }
    }


    public class EventBus : IStartableBus
    {
        private IContainer _container;
        public ISubscriptionBuilder _subscriptionBuilder;

        public EventBus(IContainer container, ISubscriptionBuilder subscriptionBuilder)
        {
            _subscriptionBuilder = subscriptionBuilder;
            _container = container;
        }

        public void Start()
        {
            Subscriptions = _subscriptionBuilder.BuildSubscriptions();
        }

        public void Publish<T>(T message)
        {
            var handlers = Subscriptions[typeof(T)];

            foreach(var handler in handlers)
            {
                ((dynamic)_container.GetService(handler)).Handle(message);
            }
        }

        public void Dispatch<T>(T entity) where T : IEntity
        {
            foreach(var @event in entity.Events)
            {
                Publish(@event);
            }
        }

        public Dictionary<Type, HashSet<Type>> Subscriptions { get; set; }
    }

    public interface ISubscriptionBuilder
    {
        Dictionary<Type, HashSet<Type>> BuildSubscriptions();
    }

    public class AssemblyScannerBuilder : ISubscriptionBuilder
    {
        Assembly[] _assemblies;

        public AssemblyScannerBuilder(params Assembly[] assemblies)
        {
            if(assemblies == null || !assemblies.Any())
            {
                _assemblies = new Assembly[]
                {
                    Assembly.GetEntryAssembly()
                };
            }
            _assemblies = assemblies;

        }

        public Dictionary<Type, HashSet<Type>> BuildSubscriptions()
        {
            var eventHandlerType = typeof(IEventHandler<,>);
            var result = new Dictionary<Type, HashSet<Type>>();

            var eventHandlers = _assemblies.SelectMany(assembly =>
                assembly.GetTypes());

            foreach (var handlerType in eventHandlers)
            {
                var handlerTypeInfo = handlerType.GetTypeInfo();
                var implementedInterface = handlerTypeInfo.ImplementedInterfaces.SingleOrDefault(i =>
                    i.IsConstructedGenericType && i.GetGenericTypeDefinition() == eventHandlerType);

                if (implementedInterface != null)
                {
                    HashSet<Type> subscriptions;

                    var eventType = implementedInterface.GetGenericArguments()[0];
                    if (result.TryGetValue(eventType, out subscriptions))
                    {
                        subscriptions.Add(handlerType);
                    }
                    else
                    {
                        result[eventType] = new HashSet<Type>(new[] { handlerType });
                    }
                }
            }

            return result;
        }
    }

    public interface IServiceContainer
    {
        T GetService<T>();
    }
}
