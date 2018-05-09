<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';
	
	$language = $_POST["language"];
	$facebook = $_POST["facebook"];
	$name_user = $_POST["name_user"];
	$email_user = $_POST["email"];
	$friends = $_POST["friends"];

    // ++ LOGIN WITH FACEBOOK ID ++
	LoginFacebook($language, $facebook, $name_user, $email_user, $friends);

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
     //  MailEncryptedPassword
     //-------------------------------------------------------------
	 function MailPassword($language_par, $email_real_par, $random_password_generated_par)
	 {
		 if ($language_par == "en")
		 {
			 $to      = $email_real_par;
			 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Access password';
			 $message = 'Thanks a lot for using Your DressMakers'. "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . 'This is your password: ' . $random_password_generated_par . "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . 'Best';
			 $headers = 'From: no-reply@yourdressmakers.com' . "<p>" .
			 'Reply-To: no-reply@yourdressmakers.com' . "<p>" .
			 'X-Mailer: PHP/' . phpversion();

			 SendGlobalEmail($GLOBALS['NON_REPLY_EMAIL_ADDRESS'], $to, $subject, $message);
		 }
		 if ($language_par == "es")
		 {
			 $to      = $email_real_par;
			 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Contraseña de acceso';
			 $message = 'Muchas gracias por utilizar la app: Tus Modistas'. "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . 'Esta es tu contraseña: ' . $random_password_generated_par . "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . 'Gracias';
			 $headers = 'From: no-reply@yourdressmakers.com' . "<p>" .
			 'Reply-To: no-reply@yourdressmakers.com' . "<p>" .
			 'X-Mailer: PHP/' . phpversion();

			 SendGlobalEmail($GLOBALS['NON_REPLY_EMAIL_ADDRESS'], $to, $subject, $message);
		 }		 
	 }		 
	
     //-------------------------------------------------------------
     //  LoginFacebook
     //-------------------------------------------------------------
     function LoginFacebook($language_par, $facebook_par, $name_par, $email_user_par, $friends_par)
     {
		$email_encrypted_relogin_case = "";
		if (strlen($email_user_par) > 1)
		{
			$email_encrypted_relogin_case = EncryptText($email_user_par);
		}
		 
		// Performing SQL Consult
		$query_user = "SELECT * FROM users WHERE facebook = '$facebook_par' OR email = '$email_encrypted_relogin_case'";
		$result_user = mysqli_query($GLOBALS['LINK_DATABASE'],$query_user) or die("Query Error::UserLoginByFacebook::Select users failed");
				
		if ($row_user = mysqli_fetch_object($result_user))
		{
			$email_user_relogin = $row_user->email;
			$password_user_relogin = DecryptText($row_user->password);
			$id_user = $row_user->id;
			$name_user_relogin = $row_user->name;
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

			$email_encrypted_updated = EncryptText($email_user_par);
			$current_time_login = GetCurrentTimestamp();
			
			// UPDATE ENERGY
			$query_update_user = "UPDATE users SET validated=1, facebook='$facebook_par', friends='$friends_par', email='$email_encrypted_updated', lastlogin=$current_time_login WHERE id = $id_user";
			$result_update_user = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user) or die("Query Error::UserLoginByFacebook::Update users part 2 failed");

			if ($result_update_user)
			{
				print "true" . $GLOBALS['PARAM_SEPARATOR'] .  $email_user_par . $GLOBALS['PARAM_SEPARATOR'] .  $password_user_relogin . $GLOBALS['PARAM_SEPARATOR'] .  $id_user . $GLOBALS['PARAM_SEPARATOR'] .  $name_user_relogin . $GLOBALS['PARAM_SEPARATOR'] .  $village_user. $GLOBALS['PARAM_SEPARATOR'] .  $mapdata_user . $GLOBALS['PARAM_SEPARATOR'] . $registerdate_user . $GLOBALS['PARAM_SEPARATOR'] . $lastlogin_user . $GLOBALS['PARAM_SEPARATOR'] . $rentstart_user . $GLOBALS['PARAM_SEPARATOR'] . $rentdays_user . $GLOBALS['PARAM_SEPARATOR'] . $scoreuser_user . $GLOBALS['PARAM_SEPARATOR'] . $scoreprovider_user . $GLOBALS['PARAM_SEPARATOR'] . $votesuser_user . $GLOBALS['PARAM_SEPARATOR'] . $votesprovider_user . $GLOBALS['PARAM_SEPARATOR'] . $validated_user . $GLOBALS['PARAM_SEPARATOR'] . $skills_user . $GLOBALS['PARAM_SEPARATOR'] . $description_user . $GLOBALS['PARAM_SEPARATOR'] . $additionalrequest_user . $GLOBALS['PARAM_SEPARATOR'] . $additionaloffer_user . $GLOBALS['PARAM_SEPARATOR'] . $banned_user;
			}	
			else
			{
				print "false";
			}
		}
		else
		{
			// ++ GET MAX ID ++
			$query_consult = "SELECT max(id) as maximumId FROM users";
			$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::UserLoginByFacebook::Select max id users failed");
			$row_consult = mysqli_fetch_object($result_consult);
			$maxIdentifier = $row_consult->maximumId;
			mysqli_free_result($result_consult);
			
			$user_id_new = $maxIdentifier + 1;
			$email_encrypted = EncryptText($email_user_par);
			$random_password_generated = rand_string(6);
			$password_encrypted = EncryptText($random_password_generated);
			$current_time_registered = GetCurrentTimestamp();
			$name_temp_user_new = substr($email_user_par,0,strrpos($email_user_par, '@'));
			$ipadress_new_user = $_SERVER['REMOTE_ADDR'];
			
			$query_insert = "INSERT INTO users VALUES ($user_id_new, '$email_encrypted', 1, '$facebook_par', '$password_encrypted', '$name_temp_user_new', '', '', -1, -1, '$friends_par', $current_time_registered, $current_time_registered, -1, -1, 0, 0, 0, 0, -1, '', '', '',0,0, '', '$ipadress_new_user', 0)";
			$result_insert = mysqli_query($GLOBALS['LINK_DATABASE'],$query_insert) or die("Query Error::UserLoginByFacebook::Insert users failed");
			
			// Free resultset
			$village_user = "";
			$mapdata_user = "";
			$registerdate_user = $current_time_registered;
			$lastlogin_user = $current_time_registered;
			$rentstart_user = -1;
			$rentdays_user = -1;
			$scoreuser_user = 0;
			$scoreprovider_user = 0;
			$votesuser_user = 0;
			$votesprovider_user = 0;
			$validated_user = 1;
			$skills_user = "";
			$description_user = "";
			$additionalrequest_user = 0;
			$additionaloffer_user = 0;
			$banned_user = 0;
			
			if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
			{
				MailPassword($language_par, $email_user_par, $random_password_generated);
				print "true" . $GLOBALS['PARAM_SEPARATOR'] . $email_user_par . $GLOBALS['PARAM_SEPARATOR'] . $random_password_generated . $GLOBALS['PARAM_SEPARATOR'] .  $user_id_new . $GLOBALS['PARAM_SEPARATOR'] .  $name_temp_user_new . $GLOBALS['PARAM_SEPARATOR'] .  $village_user. $GLOBALS['PARAM_SEPARATOR'] .  $mapdata_user . $GLOBALS['PARAM_SEPARATOR'] . $registerdate_user . $GLOBALS['PARAM_SEPARATOR'] . $lastlogin_user . $GLOBALS['PARAM_SEPARATOR'] . $rentstart_user . $GLOBALS['PARAM_SEPARATOR'] . $rentdays_user . $GLOBALS['PARAM_SEPARATOR'] . $scoreuser_user . $GLOBALS['PARAM_SEPARATOR'] . $scoreprovider_user . $GLOBALS['PARAM_SEPARATOR'] . $votesuser_user . $GLOBALS['PARAM_SEPARATOR'] . $votesprovider_user . $GLOBALS['PARAM_SEPARATOR'] . $validated_user . $GLOBALS['PARAM_SEPARATOR'] . $skills_user . $GLOBALS['PARAM_SEPARATOR'] . $description_user . $GLOBALS['PARAM_SEPARATOR'] . $additionalrequest_user . $GLOBALS['PARAM_SEPARATOR'] . $additionaloffer_user . $GLOBALS['PARAM_SEPARATOR'] . $banned_user;
			}
			else
			{
				print "false";
			}
		}
	 
		// Free resultset
		mysqli_free_result($result_user);
    }

?>
