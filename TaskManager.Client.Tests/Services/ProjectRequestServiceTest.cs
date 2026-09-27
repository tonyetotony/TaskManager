using System;
using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.Tests.Services;

[TestClass]
[TestSubject(typeof(ProjectRequestService))]
public class ProjectRequestServiceTest
{
    private AuthToken _token;
    private ProjectRequestService _service;

    public ProjectRequestServiceTest()
    {
        _token = new UsersRequestService().GetToken("admin@example.com", "qwerty123");
        _service = new ProjectRequestService();
    }

    [TestMethod]
    public void GetProjectsTest()
    {
        var projects = _service.GetProjects(_token);
        Console.WriteLine(projects.Count);
        Assert.AreNotEqual(Array.Empty<ProjectModel>(), projects.ToArray());
    }
    
    [TestMethod]
    public void GetProjectByIdTest()
    {
        var project = _service.GetProjectById(_token, 19);
        
        Assert.AreNotEqual(null, project);
    }
    
    [TestMethod]
    public void CreateProjectTest()
    {
        ProjectModel project = new ProjectModel("test", "newTestProject",ProjectStatus.InProgress, 13);
        var result = _service.CreateProject(_token, project);
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
    
    [TestMethod]
    public void UpdateProjectTest()
    {
        ProjectModel project = new ProjectModel("test updated", "newTestProject updated",ProjectStatus.Suspended, 1);
        project.Id = 21;
        var result = _service.UpdateProject(_token, project);
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
    
    [TestMethod]
    public void DeleteProjectTest()
    {
        var result = _service.DeleteProject(_token, 21);
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
    
    [TestMethod]
    public void AddUsersToProjectTest()
    {
        var result = _service.AddUsersToProject(_token, 20, new List<int>(){27,28});
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
    
    [TestMethod]
    public void RemoveUsersToProjectTest()
    {
        var result = _service.RemoveUsersToProject(_token, 20, new List<int>(){27});
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
}