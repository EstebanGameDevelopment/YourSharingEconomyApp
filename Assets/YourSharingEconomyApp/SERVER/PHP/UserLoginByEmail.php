<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';
	
	$email = $_GET["email"];
	$password = $_GET["password"];

    // ++ LOGIN WITH email ++
	LoginEmail($email, $password);

    // Closing connection
    mysqli_close($GLOBALS['LINK_DATABASE']);
          
     //**************************************************************************************
     //**************************************************************************************
     //**************************************************************************************
     // FUNCTIONS
     //**************************************************************************************
     //**************************************************************************************
     //**************************************************************************************     
	 
     //-------------------------------------------------------------
     //  LoginEmail
     //-------------------------------------------------------------
     function LoginEmail($email_par, $password_par)
     {
		$email_encrypted = EncryptText($email_par);
		$password_encrypted = EncryptText($password_par);
		 
		// Performing SQL Consult
		$query_user = "SELECT * FROM users WHERE email = '$email_encrypted' AND password = '$password_encrypted'";
		$result_user = mysqli_query($GLOBALS['LINK_DATABASE'],$query_user) or die("Query Error::UserLoginByEmail::Select users failed");
				
		if ($row_user = mysqli_fetch_object($result_user))
		{
			$id_user = $row_user->id;
			$name_user = $row_user->name;
			$village_user = $row_user->village;
			$mapdata_user = $row_user->mapdata;
			$registerdate_user = $row_user->registerdate;
			$lastlogin_user = $row_user->lastlogin;
			$rentstart_user = $row_user->rentstart;
			$rentdays_user = $row_user->rentdays;
			$scoreuser_user = $row_user->scoreuser;
			$scoreprovider_user = $row_user->scoreprovider;
			$votesuser_user = $row_user->votesuser;
			$votesprovider_user = $row_user->votesprovider;
			$validated_user = $row_user->validated;
			$skills_user = $row_user->skills;
			$description_user = $row_user->description;
			$additionalrequest_user = $row_user->additionalrequest;
			$additionaloffer_user = $row_user->additionaloffer;
			$banned_user = $row_user->banned;

			$current_time_login = GetCurrentTimestamp();
			
			// UPDATE ENERGY
			$query_update_user = "UPDATE users SET lastlogin=$current_time_login WHERE id = $id_user";
			$result_update_user = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user) or die("Query Error::UserLoginByEmail::Update users failed");

			if ($result_update_user)
			{
				print "true" . $GLOBALS['PARAM_SEPARATOR'] .  $id_user . $GLOBALS['PARAM_SEPARATOR'] . $name_user . $GLOBALS['PARAM_SEPARATOR'] . $village_user . $GLOBALS['PARAM_SEPARATOR'] . $mapdata_user . $GLOBALS['PARAM_SEPARATOR'] . $registerdate_user . $GLOBALS['PARAM_SEPARATOR'] . $lastlogin_user . $GLOBALS['PARAM_SEPARATOR'] . $rentstart_user . $GLOBALS['PARAM_SEPARATOR'] . $rentdays_user . $GLOBALS['PARAM_SEPARATOR'] . $scoreuser_user . $GLOBALS['PARAM_SEPARATOR'] . $scoreprovider_user . $GLOBALS['PARAM_SEPARATOR'] . $votesuser_user . $GLOBALS['PARAM_SEPARATOR'] . $votesprovider_user . $GLOBALS['PARAM_SEPARATOR'] . $validated_user . $GLOBALS['PARAM_SEPARATOR'] . $skills_user . $GLOBALS['PARAM_SEPARATOR'] . $description_user . $GLOBALS['PARAM_SEPARATOR'] . $additionalrequest_user . $GLOBALS['PARAM_SEPARATOR'] . $additionaloffer_user . $GLOBALS['PARAM_SEPARATOR'] . $banned_user;
			}	
			else
			{
				print "false";
			}
		}
		else
		{
			print "false";
		}
	 
		// Free resultset
		mysqli_free_result($result_user);
    }

	
	
?>
