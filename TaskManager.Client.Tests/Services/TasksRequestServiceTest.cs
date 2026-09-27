using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Client.Models;
using TaskManager.Client.Services;

namespace TaskManager.Client.Tests.Services;

[TestClass]
[TestSubject(typeof(TasksRequestService))]
public class TasksRequestServiceTest
{
    private AuthToken _token;
    private TasksRequestService _service;

    public TasksRequestServiceTest()
    {
        _token = new UsersRequestService().GetToken("admin@example.com", "qwerty123");
        _service = new TasksRequestService();
    }

    [TestMethod]
    public void GetTasksForCurrentUserTest()
    {
        var tasks = _service.GetTasksForCurrentUser(_token);
        Console.WriteLine(tasks.Count);
        Assert.AreNotEqual(0, tasks.Count);
    }
    
    [TestMethod]
    public void GetTest()
    {
        var task = _service.Get(_token, 13);
        Assert.AreNotEqual(null, task);
    }
    
    [TestMethod]
    public void GetTasksByDeskTest()
    {
        var tasks = _service.GetTasksByDesk(_token, 2);
        Assert.AreNotEqual(0, tasks.Count);
    }
}