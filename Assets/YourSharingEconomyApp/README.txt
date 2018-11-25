----------------------------------------------
        Your Sharing Economy App
	Copyright © 2018 Esteban Gallardo
            Version 2.0
   http://www.yoursharingeconomyapp.com
	 info@yoursharingeconomyapp.com
----------------------------------------------

----------------------
PLATFORM AGNOSTIC CODE
----------------------

Even if it has been developed with Unity, since it's been designed using design patterns it's easy to port this code to any exiting C++, Java, Unreal, etc...

-----------------------------------
TUTORIAL INSTALLATION
Video Tutorial: https://youtu.be/xRwAmkJTWVo
-----------------------------------

1. Import the package "Your Sharing Economy App" in a new Unity project

2. Change Scripting Runtime to .NET 4.X Equivalent

3. Import the free Facebook SDK.

	https://developers.facebook.com/docs/unity/downloads

4. Import the plugins Flat Calendar.
	
5. Open the "Windows->Services" and activate Unity In-App Purchases
	2.1. Import fromm the submenu in this panel the package of the In-App Purchases
	2.2. If there are compile error even in the IAPDemo then try to reload Services or Unity, or report a bug to Unity team.

6. Download XAMMP, install it (https://www.apachefriends.org/download.html) and start it.

7. Open phpMyAdmin, create a database called (yoursharingeconomyapp) and import the database skeleton from the file:
	
	YourSharingEconomyApp\SERVER\DATABASE\yoursharingeconomyappdb.sql
	
8. Create a folder in your server and copy all PHP files and folders from this source folder in the package:

		YourSharingEconomyApp\SERVER\PHP
	
	to your local webserver:
	
		c:\\xampp\htdocs\yoursharingeconomyapp
	
	remember the folder in the server where are the PHP must be protected by some authentication method in order to prevent attacks.
	
9. Run the application and register a new user through email

10. BASIC TUTORIAL END. You should be able to create multiple users, create requests, all the functionality available for you to use.


-----------------------------------
TUTORIAL CONFIGURATION SYSTEM APP
Video Tutorial: https://youtu.be/3dvGDQavcu8
-----------------------------------

Now let's take a look to the most basic parameters of the configuration's application.

	Assets\YourSharingEconomyApp\SERVER\PHP\ConfigurationYourSharingEconomyApp.php

We will open this php file where we will be able to customize the parameters to our needs.

We will start with the connection to the database that is this line of code. Just make sure that the database you create in PHPMyAdmin is that same.

	mysqli_select_db($LINK_DATABASE, "yoursharingeconomyapp") or die("Database Error::Could not select database)");
	
Next is the URL where all the PHP files that uses the application will be.

	// ADDRESS OF YOUR SERVER
	$URL_BASE_SERVER = "http://localhost:8081/yoursharingeconomyapp/";

Now it's time to apply an encryption algorithm to save our senstive data in the database encrypted. Just make sure that you change the name of this variable when you go to release the app.

	// DATABASE ENCRYPTION
	$private_key_aes = "TdLdsQLiaDrSQUpd";
	
Next are the tokens used in the communications to diferentiate between the different parameters. No need to change nothing here unless to want to go deeper with the changes.

	// SEPARATOR TOKENS USED IN HTTPS COMMUNICATIONS
	$PARAM_SEPARATOR = "<par>";
	$LINE_SEPARATOR = "<line>";
	
The next variable will define how many reports from the users are needed in order to automatically change the request to a warning that will advise the other users that there could be something wrong with that requests.

	// AFTER A NUMBER OF REPORTS THE REQUEST WILL SHOW A WARNING IMAGE
	$TOTAL_REPORT_REQUEST_TO_WARN_USERS = 3; 
	
In the next images you can see the diffence between a normal request and a request that has received the state of warning. When the users click on that request a warning message will advise them that the request could be toxic.

If you are not interested to keep tracking of the reports this variable will allow you to automatically set as toxic request the requests that receive a high number of reports.

	// AFTER 10 USERS REPORT A REQUEST AS TOXIC IT WILL BE AUTOMATICALLY FLAGGED
	$TOTAL_REPORT_REQUEST_TO_PENALTY = 10; 

You can see in these images how the toxic ban has been applied to the request and it will not appear in the search, but it will appear in the history linked to the user's profile so everybody can know about the toxic behavior of the user.

Next parameters is the time it has to pass in order to be able to post another proposal for a request for free. The system is prepared so the provider can not keep posting offers without stopping.

	// NUMBER OF HOURS TO WAIT TO CREATE A NEW PROPOSAL
	$HOURS_TO_WAIT_FOR_NEW_PROPOSAL = 3;
	
In the images we can see an example of two proposal posted. The first one is for free and the second one is through an Inn App Purchase. 

This next parameter is the number of free requests avaible to the customer to create requests. If the customer wants to create many requests at the same time he will have to pay for it.

	// FREE REQUESTS AVAILABLE TO THE CONSUMER
	$FREE_REQUESTS_AVAILABLE_TO_CONSUMERS = 1;

The next three parameters are related to the number of images that the users can use for the posts. Configure them wisely if you don't want that users could make you run out of space in the server.

	// TOTAL NUMBER OF IMAGES THAT CAN BE USED AS A REFERENCE (ALLOWS TO CONTROL THE DATA FOREACH REQUEST)
	$TOTAL_NUMBER_IMAGES_AS_REFERENCE = 3;

    // TOTAL NUMBER OF IMAGES THAT CAN BE USED AS A FINISHED JOB (ALLOWS TO CONTROL THE DATA FOREACH REQUEST)
	$TOTAL_NUMBER_IMAGES_AS_FINISHED = 3;
    
	// TOTAL NUMBER OF IMAGES THAT THE PROVIDER CAN POST TO SHOW HIS EXPERIENCE
	$TOTAL_NUMBER_IMAGES_PROVIDER_EXPERIENCE = 10;
	
Finally, the last one is an additional encryption system used for the In App Purchases. Please, remember to change it both in this PHP file and in the C# code of the project.

	// PASSWORD ENCRYPTION COMMS
	$kyRJEncryption = 'sK1rwpD1p+5e#bvt31CK13z77n=ES8jR'; // 32 * 8 = 256 bit key
	$ivRJEncryption = 'A9q2N2haeQybv8#Aq!N9ybc1Cnrx12@y'; // 32 * 8 = 256 bit iv	

	
-----------------------------------
TUTORIALS HOW TO USE
-----------------------------------

You can find all the tutorial related to this project in the channel:

https://www.youtube.com/channel/UCorjuU64YMzw_vavSpE7cTg

