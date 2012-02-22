//
// Copyright (c) 2012 Joe Modjeski
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
// associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
// LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ModjeskiNet
{
    [TestClass]
    public class IndependentIQueryableIncludeTests
    {
        [TestMethod]
        public void Include_On_IQueryable_Without_Include_Does_Not_Error()
        {
            var query = new MockQueryableWithoutInclude<SampleObject>().AsQueryable<SampleObject>();

            query.Include<SampleObject>("Navigation");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Include_On_IQueryable_With_Bad_Path_Throws_ArgumentException()
        {
            var queryMock = new Mock<MockQueryableWithInclude<SampleObject>>();
            var query = queryMock.Object.AsQueryable<SampleObject>();

            query.Include(o=>o.CollectionNavigation.Where(x=>x.Navigation != null));
        }

        [TestMethod]
        public void Include_On_IQueryable_With_Include_Invokes_Method()
        {
            var queryMock = new Mock<MockQueryableWithInclude<SampleObject>>();
            var query = queryMock.Object.AsQueryable<SampleObject>();

            query.Include<SampleObject>("Navigation");

            queryMock.Verify(q => q.Include("Navigation"), Times.Once());
        }

        [TestMethod]
        public void Include_On_Untyped_IQueryable_With_Include_Invokes_Method()
        {
            var queryMock = new Mock<MockQueryableWithInclude<SampleObject>>();
            var query = (IQueryable)queryMock.Object.AsQueryable();

            query.Include("Navigation");

            queryMock.Verify(q => q.Include("Navigation"), Times.Once());
        }

        [TestMethod]
        public void Include_On_Navigation_By_Expression_Invokes_Method()
        {
            var queryMock = new Mock<MockQueryableWithInclude<SampleObject>>();
            var query = queryMock.Object.AsQueryable<SampleObject>();

            query.Include(o=>o.Navigation);

            queryMock.Verify(q => q.Include("Navigation"), Times.Once());
        }

        [TestMethod]
        public void Include_On_CollectionNavigation_By_Expression_Invokes_Method()
        {
            var queryMock = new Mock<MockQueryableWithInclude<SampleObject>>();
            var query = queryMock.Object.AsQueryable<SampleObject>();

            query.Include(o => o.CollectionNavigation);

            queryMock.Verify(q => q.Include("CollectionNavigation"), Times.Once());
        }

        [TestMethod]
        public void Include_On_Nested_Navigation_By_Expression_Invokes_Method()
        {
            var queryMock = new Mock<MockQueryableWithInclude<SampleObject>>();
            var query = queryMock.Object.AsQueryable<SampleObject>();

            query.Include(o => o.Navigation.Navigation);

            queryMock.Verify(q => q.Include("Navigation.Navigation"), Times.Once());
        }

        [TestMethod]
        public void Include_On_Nested_CollectionNavigation_By_Expression_Invokes_Method()
        {
            var queryMock = new Mock<MockQueryableWithInclude<SampleObject>>();
            var query = queryMock.Object.AsQueryable<SampleObject>();

            query.Include(o => o.CollectionNavigation.Select(c=>c.Navigation));

            queryMock.Verify(q => q.Include("CollectionNavigation.Navigation"), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Include_On_Navigation_With_Invalid_CollectionNavigation_By_Expression_Invokes_Method()
        {
            var queryMock = new Mock<MockQueryableWithInclude<SampleObject>>();
            var query = queryMock.Object.AsQueryable<SampleObject>();

            query.Include(o => o.Navigation.CollectionNavigation.Where(c => c.Navigation != null));
        }
    }
}
