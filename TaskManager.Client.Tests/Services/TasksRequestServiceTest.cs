using System;
using System.Net;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

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
    
    [TestMethod]
    public void CreateTest()
    {
        TaskModel task = new TaskModel("Задача тест", "задача из тестов", DateTime.Now, DateTime.Now,"New");
        task.DeskId = 3;
        task.ExecutorId = 1;
        var result = _service.Create(_token, task);
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
    
    [TestMethod]
    public void UpdateTest()
    {
        TaskModel task = new TaskModel("Задача тест update", "задача из тестов upd", DateTime.Now, DateTime.Now,"In Progress");
        task.Id = 15;
        task.ExecutorId = 13;
        var result = _service.Update(_token, task);
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
    
    [TestMethod]
    public void DeleteTest()
    {
        var result = _service.Delete(_token, 15);
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
}