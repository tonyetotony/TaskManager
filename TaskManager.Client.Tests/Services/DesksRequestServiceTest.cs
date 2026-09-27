using System;
using System.Net;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.Tests.Services;

[TestClass]
[TestSubject(typeof(DesksRequestService))]
public class DesksRequestServiceTest
{
    private AuthToken _token;
    private DesksRequestService _service;

    public DesksRequestServiceTest()
    {
        _token = new UsersRequestService().GetToken("admin@example.com", "qwerty123");
        _service = new DesksRequestService();
    }

    [TestMethod]
    public void GetDesksForCurrentUserTest()
    {
        var desks = _service.GetDesksForCurrentUser(_token);
        Console.WriteLine(desks.Count);
        Assert.AreNotEqual(Array.Empty<DeskModel>(), desks.ToArray());
    }
    
    [TestMethod]
    public void GetTest()
    {
        var desk = _service.Get(_token, 2);
        Assert.AreNotEqual(null, desk);
    }
    
    [TestMethod]
    public void GetProjectDesksTest()
    {
        var desks = _service.GetProjectDesks(_token, 19);
        Assert.AreNotEqual(0, desks.Count);
    }
    
    [TestMethod]
    public void CreateTest()
    {
        DeskModel desk = new DeskModel("Desk for test", "new desk for test",true, new string[]{"test", "test2"});
        desk.ProjectId = 20;
        desk.AdminId = 13;
        var result = _service.Create(_token, desk);
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
    
    [TestMethod]
    public void UpdateTest()
    {
        DeskModel desk = new DeskModel("Desk for test", "new desk for test",true, new string[]{"test","test1", "test2"});
        desk.ProjectId = 20;
        desk.AdminId = 13;
        desk.Id = 5;
        var result = _service.Update(_token, desk);
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
    
    [TestMethod]
    public void DeleteTest()
    {
        var result = _service.Delete(_token, 5);
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
}