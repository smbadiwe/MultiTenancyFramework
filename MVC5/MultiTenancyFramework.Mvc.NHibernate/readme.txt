Thank you for installing the MultiTenancy Framework using MVC5 and NHibernate!
You can now easily build SaaS using MVC5 and NHibernate.
Learn more on https://github.com/smbadiwe/MultiTenancyFramework/wiki

A few things to note:
- Your config file will be modified. Here are the ones that need your attention:
	- The NHibernate section. Modify accordingly 
	- AppSettings: the key 'SiteUrl' added. Modify the URL accordingly
	- AppSettings: the key 'EntityAssemblies' where you may add the assembly names (comma separated) of the assemblies where your entities reside

Feel free to contriute, raise an issue, suggest improvements or provide general feedback on the Github page for the framework