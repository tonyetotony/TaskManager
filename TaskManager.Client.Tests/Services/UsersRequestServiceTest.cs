using System;
using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.Tests.Services;

[TestClass]
[TestSubject(typeof(UsersRequestService))]
public class UsersRequestServiceTest
{

    [TestMethod]
    public void GetTokenTest()
    {
        var token = new UsersRequestService().GetToken("admin@example.com", "qwerty123");
        Console.WriteLine(token.access_token);
        Assert.IsNotNull(token);
        
    }
    
    [TestMethod]
    public void CreateUserTest()
    {
        var service = new UsersRequestService();
        var token = service.GetToken("admin@example.com", "qwerty123");
        UserModel userTest = new UserModel( "Mary", "Gray", "marygray@mail.ru", "qwerty123", UserStatus.User, "79991234567", null);
        var result = service.CreateUser(token, userTest);
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
    
    [TestMethod]
    public void GetAllUsersTest()
    {
        var service = new UsersRequestService();
        var token = service.GetToken("admin@example.com", "qwerty123");
        var result = service.GetAllUsers(token);
        Console.WriteLine(result.Count);
        Assert.AreNotEqual(Array.Empty<UserModel>(), result.ToArray());
    }
    
    [TestMethod]
    public void DeleteUserTest()
    {
        var service = new UsersRequestService();
        var token = service.GetToken("admin@example.com", "qwerty123");
        var result = service.DeleteUser(token, 24);
        
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
    
    [TestMethod]
    public void CreateUsersTest()
    {
        var service = new UsersRequestService();
        var token = service.GetToken("admin@example.com", "qwerty123");
        UserModel userTest1 = new UserModel( "Walter", "White", "heisenberg@mail.ru", "qwerty123", UserStatus.User, "79991112233", null);
        UserModel userTest2 = new UserModel( "Jesse", "Pinkman", "pinkman@mail.ru", "qwerty123", UserStatus.User, "79992223344", null);
        List<UserModel> users = new List<UserModel>(){userTest1, userTest2};
        var result = service.CreateUsers(token, users);
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
    
    [TestMethod]
    public void UpdateUserTest()
    {
        var service = new UsersRequestService();
        var token = service.GetToken("admin@example.com", "qwerty123");
        UserModel updatableUser = new UserModel( "Walter", "White", "heisenberg@mail.ru", "qwerty123", UserStatus.Admin, "79999999999", null);
        updatableUser.Id = 27;
        var result = service.UpdateUser(token, updatableUser);
        
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
}