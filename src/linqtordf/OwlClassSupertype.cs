/* 
 * Copyright (C) 2007, Andrew Matthews http://aabs.wordpress.com/
 *
 * This file is Free Software and part of LinqToRdf http://code.google.com/p/linqtordf/
 *
 * It is licensed under the following license:
 *   - Berkeley License, V2.0 or any newer version
 *
 * You may not use this file except in compliance with the above license.
 *
 * See http://code.google.com/p/linqtordf/ for the complete text of the license agreement.
 *
 */
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LinqToRdf
{
    public class OwlClassSupertype
    {
        private static bool IsPersistentProperty(PropertyInfo pi)
        {
            return pi.GetCustomAttributes(typeof(OwlPropertyAttribute), true).Length > 0;
        }
        public IEnumerable<PropertyInfo> AllPersistentProperties
        {
            get
            {
                foreach (PropertyInfo propertyInfo in GetType().GetProperties())
                {
                    if (IsPersistentProperty(propertyInfo))
                    {
                        yield return propertyInfo;
                    }
                }
            }
        }
        public static IEnumerable<PropertyInfo> GetAllPersistentProperties(Type t)
        {
            foreach (PropertyInfo propertyInfo in t.GetProperties())
            {
                if (IsPersistentProperty(propertyInfo))
                {
                    yield return propertyInfo;
                }
            }
        }

        public static string GetOntologyBaseUri(Type t)
        {
            OntologyBaseUriAttribute[] baseUris =
                (OntologyBaseUriAttribute[])t.GetCustomAttributes(typeof(OntologyBaseUriAttribute), true);
            if (baseUris == null || baseUris.Length == 0 || baseUris[0] == null)
                throw new ApplicationException("No ontology baseUri attribute has been set");
            return baseUris[0].BaseUri;
        }

        public static string GetOwlClassUri(Type t)
        {
            OwlClassAttribute[] ta = (OwlClassAttribute[])t.GetCustomAttributes(typeof(OwlClassAttribute), true);
            if (ta[0].IsRelativeUri)
                return GetOntologyBaseUri(t) + ta[0].Uri;
            else
                return ta[0].Uri;
        }

        public static string GetInstanceBaseUri(Type t)
        {
            OwlClassAttribute[] classNames =
                (OwlClassAttribute[])t.GetCustomAttributes(typeof(OwlClassAttribute), true);
            if (classNames == null || classNames.Length == 0 || classNames[0] == null)
                throw new ApplicationException("no owl class name attribute has been set");
            if (classNames[0].IsRelativeUri)
                return GetOntologyBaseUri(t) + classNames[0].Uri;
            else
                return classNames[0].Uri;
        }

		public static string GetPropertyUri(Type t, string propName)
		{
			return GetPropertyUri(t, propName, false);
		}

		public static string GetPropertyUri(Type t, string propName, bool giveRelativeUri)
		{
			PropertyInfo pi = t.GetProperty(propName);
			OwlPropertyAttribute[] props = (OwlPropertyAttribute[])pi.GetCustomAttributes(typeof(OwlPropertyAttribute), false);
			if (props == null || props.Length == 0)
				throw new ApplicationException("No OwlPropertyAttribute has been added to property " + propName);
			if (!props[0].IsRelativeUri)
				return props[0].Uri;
			return (giveRelativeUri?"":GetOntologyBaseUri(t)) + props[0].Uri;
		}
	}
}