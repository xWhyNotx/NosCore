﻿using Autofac;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Configuration;
using NosCore.Configuration;
using NosCore.Controllers;
using NosCore.Core;
using NosCore.Core.Logger;
using NosCore.Core.Serializing;
using NosCore.Packets.ClientPackets;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NosCore.WorldServer
{
    public static class WorldServerBootstrap
    {
        private const string _configurationPath = @"..\..\..\configuration";

        private static void PrintHeader()
        {
            Console.Title = "NosCore - WorldServer";
            const string text = "WORLD SERVER - 0Lucifer0";
            int offset = (Console.WindowWidth / 2) + (text.Length / 2);
            string separator = new string('=', Console.WindowWidth);
            Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);
        }

        private static WorldConfiguration InitializeConfiguration()
        {
            var builder = new ConfigurationBuilder();
            WorldConfiguration worldConfiguration = new WorldConfiguration();
            builder.SetBasePath(Directory.GetCurrentDirectory() + _configurationPath);
            builder.AddJsonFile("world.json", false);
            builder.Build().Bind(worldConfiguration);
            Logger.Log.Info(LogLanguage.Instance.GetMessageFromKey(LogLanguageKey.SUCCESSFULLY_LOADED));
            return worldConfiguration;
        }

        private static void InitializeLogger()
        {
            // LOGGER
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("../../configuration/log4net.config"));
            Logger.InitializeLogger(LogManager.GetLogger(typeof(WorldServer)));
        }

        private static void InitializePackets()
        {
            PacketFactory.Initialize<NoS0575Packet>();
        }

        public static void Main()
        {
            PrintHeader();
            InitializeLogger();
            var container = InitializeContainer();
            var worldServer = container.Resolve<WorldServer>();
            worldServer.Run();
        }

        private static IContainer InitializeContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(InitializeConfiguration()).As<WorldConfiguration>();
            containerBuilder.RegisterAssemblyTypes(typeof(DefaultPacketController).Assembly)
              .Where(t => t.IsAssignableFrom(typeof(PacketController)))
              .AsImplementedInterfaces().InstancePerRequest();
            containerBuilder.RegisterType<WorldServer>().PropertiesAutowired();
            return containerBuilder.Build();
        }
    }
}