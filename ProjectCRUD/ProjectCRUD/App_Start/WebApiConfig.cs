using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ProjectCRUD.Infrastructure;
using ProjectCRUD.Services;
using ProjectCRUD.Models;
using Unity;
using System.Configuration;
using MongoDB.Driver;

namespace ProjectCRUD
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //Database related
            var connectionString = ConfigurationManager.AppSettings["MongoDBConectionString"];
            var client = new MongoClient(connectionString);
            var databaseName = ConfigurationManager.AppSettings["MongoDBDatabaseName"];
            var collectionName = ConfigurationManager.AppSettings["MongoDBCollectionName"];
            var database = client.GetDatabase(databaseName);
            var IMongoCol = database.GetCollection<Student>(collectionName);

            var container = new UnityContainer();
            container.RegisterType<IStudentService, StudentService>();
            container.RegisterInstance<IMongoCollection<Student>>(IMongoCol);
            config.DependencyResolver = new UnityResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
