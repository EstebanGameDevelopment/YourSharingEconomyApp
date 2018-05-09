<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$language = $_GET["language"];
	$email = $_GET["email"];
	$password = $_GET["password"];

    // ++ LOGIN WITH email ++
	RegisterEmail($language, $email, $password);

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
     //  MailPassword
     //-------------------------------------------------------------
	 function MailPassword($language_par, $email_real_par, $random_password_generated_par, $id_par, $random_code_validate_par)
	 {
		 if ($language_par == "en")
		 {
			 // EMAIL LEVEL
			 $to      = $email_real_par;
			 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Access password';
			 $message = 'Thanks a lot for using Your DressMakers'. "<p>";
			 $message = $message . "<p>";
			 $message = $message . 'This is your password: ' . $random_password_generated_par . "<p>"; 
			 $message = $message . "<p>";
			 $url_link = $GLOBALS['URL_BASE_SERVER'] .'ValidateEmail.php?language=' . $language_par . '&id=' .$id_par .'&code=' . $random_code_validate_par;
			 $a_url_link = "<a href=\"" . $url_link ."\">". $url_link . "</a>";
			 $message = $message . 'Please click the next link to validate your account: ' . $a_url_link; 
			 $message = $message . "<p>";
			 $message = $message . "<p>";
			 $message = $message . 'Best';
			 $headers = 'From: no-reply@yourdressmakers.com' . "<p>" .
			 'Reply-To: no-reply@yourdressmakers.com' . "<p>" .
			 'X-Mailer: PHP/' . phpversion();

			 SendGlobalEmail($GLOBALS['NON_REPLY_EMAIL_ADDRESS'], $to, $subject, $message);
		 }
		 if ($language_par == "es")
		 {
			 // EMAIL LEVEL
			 $to      = $email_real_par;
			 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Contraseña de acceso';
			 $message = 'Muchas gracias por utilizar la app: Tus Modistas'. "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . 'Esta es tu contraseña: ' . $random_password_generated_par . "<p>"; 
			 $message = $message . "<p>";
			 $url_link = $GLOBALS['URL_BASE_SERVER'] .'ValidateEmail.php?language=' . $language_par . '&id=' .$id_par .'&code=' . $random_code_validate_par;
			 $a_url_link = "<a href=\"" . $url_link ."\">". $url_link . "</a>";
			 $message = $message . 'Por favor pulsa en el enlace para confirmar tu cuenta: ' . $a_url_link; 
			 $message = $message . "<p>";
			 $message = $message . 'Gracias';
			 $headers = 'From: no-reply@yourdressmakers.com' . "<p>" .
			 'Reply-To: no-reply@yourdressmakers.com' . "<p>" .
			 'X-Mailer: PHP/' . phpversion();

			 SendGlobalEmail($GLOBALS['NON_REPLY_EMAIL_ADDRESS'], $to, $subject, $message);
		 }
	 }		 
	
     //-------------------------------------------------------------
     //  RegisterEmail
     //-------------------------------------------------------------
     function RegisterEmail($language_par, $email_par, $password_par)
     {
		$query_user = "SELECT id FROM users WHERE email = '$email_par'";
		$result_user = mysqli_query($GLOBALS['LINK_DATABASE'],$query_user) or die("Query Error::UserRegisterByEmail::Select users failed");
				
		if ($row_user = mysqli_fetch_object($result_user))
		{
			// USER ALREADY EXISTS
			print "false";
		}
		else
		{
			$query_consult = "SELECT max(id) as maximumId FROM users";
			$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::UserRegisterByEmail::Select max users failed");
			$row_consult = mysqli_fetch_object($result_consult);
			$maxIdentifier = $row_consult->maximumId;
			mysqli_free_result($result_consult);
			
			$user_id_new = $maxIdentifier + 1;
			$email_encrypted = EncryptText($email_par);
			$password_encrypted = EncryptText($password_par);
			$current_time_registered = GetCurrentTimestamp();
			$name_temp_user_new = substr($email_par,0,strrpos($email_par, '@'));
			$random_code_validate = rand_string(6);
			$ipadress_new_user = $_SERVER['REMOTE_ADDR'];
	
			// UNCOMMENT THIS LIKE WHEN YOU ARE WORKING WITH A VALID MAIL SERVER ABLE TO DISPATCH EMAILS
			// $query_insert = "INSERT INTO users VALUES ($user_id_new, '$email_encrypted', -1, '', '$password_encrypted', '$name_temp_user_new', '', '', -1, -1, '', $current_time_registered, $current_time_registered, -1, -1, 0, 0, 0, 0, -1, '$random_code_validate','','',0,0, '', '$ipadress_new_user', 0)";
			$query_insert = "INSERT INTO users VALUES ($user_id_new, '$email_encrypted', 1, '', '$password_encrypted', '$name_temp_user_new', '', '', -1, -1, '', $current_time_registered, $current_time_registered, -1, -1, 0, 0, 0, 0, -1, '','','',0,0, '', '$ipadress_new_user', 0)";
			
			$result_insert = mysqli_query($GLOBALS['LINK_DATABASE'],$query_insert) or die("Query Error::UserRegisterByEmail::Insert users failed");
			
			if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
			{
				MailPassword($language_par, $email_par, $password_par, $user_id_new, $random_code_validate);
				
				$name_user = $email_par;
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
				$validated_user = -1;
				$skills_user = "";
				$description_user = "";
				$additionalrequest_user = 0;
				$additionaloffer_user = 0;
				$banned_user = 0;
				print "true" . $GLOBALS['PARAM_SEPARATOR'] .  $user_id_new . $GLOBALS['PARAM_SEPARATOR'] .  $name_user. $GLOBALS['PARAM_SEPARATOR'] .  $village_user. $GLOBALS['PARAM_SEPARATOR'] .  $mapdata_user . $GLOBALS['PARAM_SEPARATOR'] . $registerdate_user . $GLOBALS['PARAM_SEPARATOR'] . $lastlogin_user . $GLOBALS['PARAM_SEPARATOR'] . $rentstart_user . $GLOBALS['PARAM_SEPARATOR'] . $rentdays_user . $GLOBALS['PARAM_SEPARATOR'] . $scoreuser_user . $GLOBALS['PARAM_SEPARATOR'] . $scoreprovider_user . $GLOBALS['PARAM_SEPARATOR'] . $votesuser_user . $GLOBALS['PARAM_SEPARATOR'] . $votesprovider_user . $GLOBALS['PARAM_SEPARATOR'] . $validated_user . $GLOBALS['PARAM_SEPARATOR'] . $skills_user . $GLOBALS['PARAM_SEPARATOR'] . $description_user . $GLOBALS['PARAM_SEPARATOR'] . $additionalrequest_user . $GLOBALS['PARAM_SEPARATOR'] . $additionaloffer_user . $GLOBALS['PARAM_SEPARATOR'] . $banned_user;
			}
		}
	 
		// Free resultset
		mysqli_free_result($result_user);
    }

?>
