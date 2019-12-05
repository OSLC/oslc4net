/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *  
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *
 * Contributors:
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client.Oslc.Resources
{
    [OslcResourceShape(title = "Architecture Management LinkType Resource Shape", describes = new string[] { ArchitectureConstants.TYPE_ARCHITECTURE_LINK_TYPE })]
    [OslcNamespace(ArchitectureConstants.ARCHITECTURE_NAMESPACE)]
    /// <summary>
    /// http://open-services.net/wiki/architecture-management/OSLC-Architecture-Management-Specification-Version-2.0/
    /// </summary>
    public class ArchitectureLinkType : AbstractResource
    {
	    private readonly ISet<Uri>      contributors                = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri>      creators                    = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri>      rdfTypes                    = new HashSet<Uri>(); // XXX - TreeSet<> in Java

    
        private DateTime?   created;
        private string comment;
        private string label;
        private string identifier;
        private Uri         instanceShape;
        private DateTime?   modified;
        private Uri         serviceProvider;
    

	    public ArchitectureLinkType() : base()
	    {
		
		    rdfTypes.Add(new Uri(ArchitectureConstants.TYPE_ARCHITECTURE_LINK_TYPE));
	    }
	
        public ArchitectureLinkType(Uri about) : base(about)
         {
		    rdfTypes.Add(new Uri(ArchitectureConstants.TYPE_ARCHITECTURE_LINK_TYPE));
         }

        protected Uri GetRdfType() {
    	    return new Uri(ArchitectureConstants.TYPE_ARCHITECTURE_LINK_TYPE);
        }
    
        public void AddContributor(Uri contributor)
        {
            contributors.Add(contributor);
        }

        public void AddCreator(Uri creator)
        {
            creators.Add(creator);
        }
    
        public void AddRdfType(Uri rdfType)
        {
            rdfTypes.Add(rdfType);
        }

        [OslcDescription("The person(s) who are responsible for the work needed to complete the automation plan.")]
        [OslcName("contributor")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "contributor")]
        [OslcRange(QmConstants.TYPE_PERSON)]
        [OslcTitle("Contributors")]
        public Uri[] GetContributors()
        {
            return contributors.ToArray();
        }

        [OslcDescription("Timestamp of resource creation.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "created")]
        [OslcReadOnly]
        [OslcTitle("Created")]
        public DateTime? GetCreated()
        {
            return created;
        }

        [OslcDescription("Creator or creators of resource.")]
        [OslcName("creator")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "creator")]
        [OslcRange(ArchitectureConstants.TYPE_PERSON)]
        [OslcTitle("Creators")]
        public Uri[] GetCreators()
        {
            return creators.ToArray();
        }

        [OslcDescription("The human readable name for this link type.")]
        [OslcPropertyDefinition(OslcConstants.RDFS_NAMESPACE + "label")]
        [OslcTitle("Label")]
        [OslcOccurs(Occurs.ExactlyOne)]
        public string GetLabel()
        {
            return label;
        }
    
        [OslcDescription("Descriptive text about link type. ")]
        [OslcPropertyDefinition(OslcConstants.RDFS_NAMESPACE + "comment")]
        [OslcTitle("Comment")]
        [OslcOccurs(Occurs.ZeroOrOne)]
        public string GetComment()
        {
            return comment;
        }

        [OslcDescription("A unique identifier for a resource. Assigned by the service provider when a resource is created. Not intended for end-user display.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "identifier")]
        [OslcReadOnly]
        [OslcTitle("Identifier")]
        public string GetIdentifier()
        {
            return identifier;
        }

        [OslcDescription("Resource Shape that provides hints as to resource property value-types and allowed values. ")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "instanceShape")]
        [OslcRange(OslcConstants.TYPE_RESOURCE_SHAPE)]
        [OslcTitle("Instance Shape")]
        public Uri GetInstanceShape()
        {
            return instanceShape;
        }

        [OslcDescription("Timestamp last latest resource modification.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "modified")]
        [OslcReadOnly]
        [OslcTitle("Modified")]
        public DateTime? GetModified()
        {
            return modified;
        }

        [OslcDescription("The resource type URIs.")]
        [OslcName("type")]
        [OslcPropertyDefinition(OslcConstants.RDF_NAMESPACE + "type")]
        [OslcTitle("Types")]
        public Uri[] GetRdfTypes()
        {
            return rdfTypes.ToArray();
        }

        [OslcDescription("The scope of a resource is a Uri for the resource's OSLC Service Provider.")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "serviceProvider")]
        [OslcRange(OslcConstants.TYPE_SERVICE_PROVIDER)]
        [OslcTitle("Service Provider")]
        public Uri GetServiceProvider()
        {
            return serviceProvider;
        }
   
        public void SetContributors(Uri[] contributors)
        {
            this.contributors.Clear();

            if (contributors != null)
            {
                this.contributors.AddAll(contributors);
            }
        }

        public void SetCreated(DateTime? created)
        {
            this.created = created;
        }

        public void SetCreators(Uri[] creators)
        {
            this.creators.Clear();

            if (creators != null)
            {
                this.creators.AddAll(creators);
            }
        }

        public void SetLabel(string label)
        {
            this.label = label;
        }
    
        public void SetComment(string comment)
        {
            this.comment = comment;
        }

        public void SetIdentifier(string identifier)
        {
            this.identifier = identifier;
        }
    
        public void SetInstanceShape(Uri instanceShape)
        {
            this.instanceShape = instanceShape;
        }

        public void SetModified(DateTime? modified)
        {
            this.modified = modified;
        }

        public void SetRdfTypes(Uri[] rdfTypes)
        {
            this.rdfTypes.Clear();

            if (rdfTypes != null)
            {
                this.rdfTypes.AddAll(rdfTypes);
            }
        }
    
        public void SetServiceProvider(Uri serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
    }
}
