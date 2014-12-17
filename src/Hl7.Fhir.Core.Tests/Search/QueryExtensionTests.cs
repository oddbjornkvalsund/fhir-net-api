﻿/* 
 * Copyright (c) 2014, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/ewoutkramer/fhir-net-api/master/LICENSE
 */

using Hl7.Fhir.Model;
using Hl7.Fhir.Search;
using Hl7.Fhir.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hl7.Fhir.Test.Search
{
    [TestClass]
#if PORTABLE45
	public class PortableQueryExtensionTests
#else
    public class QueryExtensionTests
#endif
    {
        [TestMethod]
        public void ManageSearchResult()
        {
            var q = new Parameters()
                .For("Patient").Where("name:exact=ewout").OrderBy("birthDate", SortOrder.Descending)
                .SummaryOnly().Include("Patient.managingOrganization")
                .LimitTo(20);

            var x = FhirSerializer.SerializeResourceToXml(q);
            Console.WriteLine(x);

            Assert.AreEqual("Patient", q.ResourceSearchType);

            var p = q.Parameter.SingleWithName("name");
            Assert.AreEqual("name:exact", p.Name);
            Assert.AreEqual("ewout", Parameters.ExtractParamValue(p));

            var o = q.Sort;
            Assert.AreEqual("birthDate", o.First().Item1);
            Assert.AreEqual(SortOrder.Descending, o.First().Item2);

            Assert.IsTrue(q.Summary);
            Assert.IsTrue(q.Includes.Contains("Patient.managingOrganization"));
            Assert.AreEqual(20, q.Count);
        }

        [TestMethod]
        public void ReapplySingleParam()
        {
            var q = new Parameters()
                .Custom("mySearch").OrderBy("adsfadf").OrderBy("q", SortOrder.Descending)
                    .LimitTo(10).LimitTo(20).Custom("miSearch").SummaryOnly().SummaryOnly(false);

            Assert.AreEqual("miSearch", q.QueryName);
            Assert.IsFalse(q.Summary);

            var o = q.Sort;
            Assert.AreEqual("adsfadf", o.First().Item1);
            Assert.AreEqual(SortOrder.Ascending, o.First().Item2);
            Assert.AreEqual("q", o.Skip(1).First().Item1);
            Assert.AreEqual(SortOrder.Descending, o.Skip(1).First().Item2);

            Assert.AreEqual(20, q.Count);

            Assert.IsFalse(q.Summary);
        }
    }
}
