Multiple tenant feature has been added into Remote monitoring solution. You might assign users into different roles. It will be more useful to develop customized solutions.


Think about this scenario. In a training course, the trainer acts as a super admin and the students should be users. The student can’t modify the default configurations of solution and can’t view the devices created by other students to prevent the false operations.  

Meanwhile, the trainer can monitor the student’s behaviors. Such as how many new devices created by students and who is the device filters’ owner.

This doc will show you how to config this feature and how it works.


### Enable multiple tenant feature
After deploying the Remote Monitoring, following steps will enable multiple tenant feature:

- Open the azure management portal, navigate to the app service of your Remote Monitoring solution. Notice the app service’s name is the same as your Remote Monitoring solution’s name.
- Navigate to **Settings**->**Application settings** and add the super admin account to the SuperAdminList. If there are more than one super admin account, add ‘;’ between the accounts.
- Save the changes and restart your app services.
![][img-superadminlist]

### The definition of roles
There are three roles for multiple tenant feature: Superadmin, admins and guests.

- Super admin role has the full privilege for Remote Monitoring. 
	- Manage all the simulated devices.
	- Manage all the devices created by other admins.
	- Create common filters for admins.
	- Manage advanced features, such as rules’ actions, API Registration and Device Association.
	- Manage the device columns setting for all admins.
- 	Admins role only has the privileges that depends on the devices created by admin’s own.
	- 	Manage the devices created by themselves.
	- 	Schedule job, manage device’s rules and other relevant activities for the devices created by themselves.
- 	Guest role can’t view any data or records of the Remote Monitoring.


### Details

**For Admins**:

- Admins only can view the statistic numbers for their own devices on the dashboard.
 ![][img-dashboard]
- Admins might use the common filters created by Superadmin.
 ![][img-filter]

**For Superadmin**:

- Super admin can identify the devices’ owner by viewing the USERNAME column in device list.
 ![][img-username]
- Super admin can identify the filter’s owner by viewing the information icon on filter dropdown list.
 ![][img-filterowner]



<!-- Images and links -->
[img-superadminlist]: media/image5.png
[img-username]: media/image6.png
[img-filterowner]: media/image7.png
[img-dashboard]:media/image8.png
[img-filter]:media/image9.png