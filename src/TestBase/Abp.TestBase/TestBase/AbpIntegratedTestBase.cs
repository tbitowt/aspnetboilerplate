﻿using System;
using Abp.Collections;
using Abp.Dependency;
using Abp.Modules;
using Abp.Runtime.Session;
using Abp.TestBase.Modules;
using Abp.TestBase.Runtime.Session;

namespace Abp.TestBase
{
    /// <summary>
    /// This is the base class for all tests integrated to ABP.
    /// </summary>
    public abstract class AbpIntegratedTestBase : IDisposable
    {
        /// <summary>
        /// A reference to the <see cref="IIocManager"/> used for this test.
        /// </summary>
        protected IIocManager LocalIocManager { get; private set; }

        /// <summary>
        /// Gets Session object. Can be used to change current user and tenant in tests.
        /// </summary>
        protected TestAbpSession AbpSession { get; private set; }

        private readonly AbpBootstrapper _bootstrapper;

        protected AbpIntegratedTestBase()
        {
            LocalIocManager = new IocManager();

            LocalIocManager.Register<IModuleFinder, TestModuleFinder>();
            LocalIocManager.Register<IAbpSession, TestAbpSession>();

            AddModules(LocalIocManager.Resolve<TestModuleFinder>().Modules);

            PreInitialize();

            _bootstrapper = new AbpBootstrapper(LocalIocManager);
            _bootstrapper.Initialize();

            AbpSession = LocalIocManager.Resolve<TestAbpSession>();
        }

        protected virtual void AddModules(ITypeList<AbpModule> modules)
        {
            modules.Add<TestBaseModule>();
        }

        /// <summary>
        /// This method can be overrided to replace some services with fakes.
        /// </summary>
        protected virtual void PreInitialize()
        {

        }

        public virtual void Dispose()
        {
            _bootstrapper.Dispose();
            LocalIocManager.Dispose();
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// Also registers <see cref="T"/> as transient if it's not registered before.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <returns>The object instance</returns>
        protected T Resolve<T>()
        {
            RegisterIfNotRegisteredBefore(typeof(T));
            return LocalIocManager.Resolve<T>();
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// Also registers <see cref="T"/> as transient if it's not registered before.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        protected T Resolve<T>(object argumentsAsAnonymousType)
        {
            RegisterIfNotRegisteredBefore(typeof(T));
            return LocalIocManager.Resolve<T>(argumentsAsAnonymousType);
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// Also registers <see cref="type"/> as transient if it's not registered before.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <returns>The object instance</returns>
        protected object Resolve(Type type)
        {
            RegisterIfNotRegisteredBefore(type);
            return LocalIocManager.Resolve(type);
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// Also registers <see cref="type"/> as transient if it's not registered before.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        protected object Resolve(Type type, object argumentsAsAnonymousType)
        {
            RegisterIfNotRegisteredBefore(type);
            return LocalIocManager.Resolve(type, argumentsAsAnonymousType);
        }

        /// <summary>
        /// Registers given type if it's not registered before.
        /// </summary>
        /// <param name="type">Type to check and register</param>
        /// <param name="lifeStyle">Lifestyle</param>
        protected void RegisterIfNotRegisteredBefore(Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Transient)
        {
            if (!LocalIocManager.IsRegistered(type))
            {
                LocalIocManager.Register(type, lifeStyle);
            }
        }
    }
}
