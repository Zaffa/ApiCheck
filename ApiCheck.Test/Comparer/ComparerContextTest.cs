﻿using System;
using ApiCheck.Comparer;
using ApiCheck.Result;
using ApiCheck.Test.Builder;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using ApiCheck.Configuration;

namespace ApiCheck.Test.Comparer
{
  class ComparerContextTest
  {
    [Test]
    public void When_type_is_changed_and_ignored_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi("A").Class("A.C").Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi("A").Build();
      IComparerResult sut = new Builder(assembly1, assembly2).Ignore("A.C").Build();

      Assert.AreEqual(0, sut.RemovedItems.Count());
    }

    [Test]
    public void When_interface_extension_is_removed_and_ignored_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi().Interface(interfaces: new[] { typeof(IDisposable) }).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi().Interface().Build().Build();
      var sut = new Builder(assembly1, assembly2).Ignore("System.IDisposable").Build();

      Assert.AreEqual(0, sut.ComparerResults.First().RemovedItems.Count());
      Assert.AreEqual(0, sut.ComparerResults.First().AddedItems.Count());
    }

    [Test]
    public void When_method_is_changed_and_ignored_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi("A").Class("A.C").Method("M").Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi("A").Class("A.C").Method("M2").Build().Build().Build();
      IComparerResult sut = new Builder(assembly1, assembly2).Ignore("A.C.M").Build();

      Assert.AreEqual(0, sut.ComparerResults.First().RemovedItems.Count());
      Assert.AreEqual(1, sut.ComparerResults.First().AddedItems.Count());
    }

    [Test]
    public void When_generic_method_is_changed_and_ignored_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi("A").Class("A.C").Method("M").Build().Method("M").GenericParameter().Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi("A").Class("A.C").Method("M2").Build().Method("M2").GenericParameter().Build().Build().Build();
      IComparerResult sut = new Builder(assembly1, assembly2).Ignore("A.C.M").Build();

      Assert.AreEqual(0, sut.ComparerResults.First().RemovedItems.Count());
      Assert.AreEqual(2, sut.ComparerResults.First().AddedItems.Count());
    }

    [Test]
    public void When_ctor_is_changed_and_ignored_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi("A").Class("A.C").Constructor().Parameter(typeof(int)).Build().Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi("A").Class("A.C").Build().Build();
      IComparerResult sut = new Builder(assembly1, assembly2).Ignore("A.C.ctor").Build();

      Assert.AreEqual(0, sut.ComparerResults.First().RemovedItems.Count());
    }

    [Test]
    public void When_event_is_changed_and_ignored_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi("A").Class("A.C").Property("P", typeof(int)).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi("A").Class("A.C").Build().Build();
      IComparerResult sut = new Builder(assembly1, assembly2).Ignore("A.C.P").Build();

      Assert.AreEqual(0, sut.ComparerResults.First().RemovedItems.Count());
    }

    [Test]
    public void When_field_is_changed_and_ignored_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi("A").Class("A.C").Field("F", typeof(int)).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi("A").Class("A.C").Build().Build();
      IComparerResult sut = new Builder(assembly1, assembly2).Ignore("A.C.F").Build();

      Assert.AreEqual(0, sut.ComparerResults.First().RemovedItems.Count());
    }

    [Test]
    public void When_property_is_changed_and_ignored_should_not_report()
    {
      Assembly assembly1 = ApiBuilder.CreateApi("A").Class("A.C").Property("P", typeof(int)).Build().Build();
      Assembly assembly2 = ApiBuilder.CreateApi("A").Class("A.C").Build().Build();
      IComparerResult sut = new Builder(assembly1, assembly2).Ignore("A.C.P").Build();

      Assert.AreEqual(0, sut.ComparerResults.First().RemovedItems.Count());
    }

    private class Builder
    {
      private readonly IComparer _comparer;
      private readonly ComparerConfiguration _configuration;

      public Builder(Assembly assembly1, Assembly assembly2)
      {
        _configuration = new ComparerConfiguration();
        _comparer = new AssemblyComparer(assembly1, assembly2, new ComparerContext(s => { }, s => { }, _configuration));
      }

      public Builder Ignore(string element)
      {
        _configuration.Ignore.Add(element);
        return this;
      }

      public IComparerResult Build()
      {
        return _comparer.Compare();
      }
    }
  }
}
