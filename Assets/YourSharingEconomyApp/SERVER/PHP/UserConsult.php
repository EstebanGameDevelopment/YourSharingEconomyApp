<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';
 
	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$user = $_GET["user"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		ConsultUser($user);
	}
	else
	{
		print "false";
	}

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
     //  ConsultRequest
     //-------------------------------------------------------------
     function ConsultUser($user_par)
     {
		// ++ GET MAX ID ++
		$query_consult = "SELECT * FROM users WHERE id = $user_par";
		$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::UserConsult::Select users failed");

		$package = "";
		if ($row_user = mysqli_fetch_object($result_consult))
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

			$output_packet = "true" . $GLOBALS['PARAM_SEPARATOR'] .  $id_user . $GLOBALS['PARAM_SEPARATOR'] . $name_user . $GLOBALS['PARAM_SEPARATOR'] . $village_user . $GLOBALS['PARAM_SEPARATOR'] . $mapdata_user . $GLOBALS['PARAM_SEPARATOR'] . $registerdate_user . $GLOBALS['PARAM_SEPARATOR'] . $lastlogin_user . $GLOBALS['PARAM_SEPARATOR'] . $rentstart_user . $GLOBALS['PARAM_SEPARATOR'] . $rentdays_user . $GLOBALS['PARAM_SEPARATOR'] . $scoreuser_user . $GLOBALS['PARAM_SEPARATOR'] . $scoreprovider_user . $GLOBALS['PARAM_SEPARATOR'] . $votesuser_user . $GLOBALS['PARAM_SEPARATOR'] . $votesprovider_user . $GLOBALS['PARAM_SEPARATOR'] . $validated_user . $GLOBALS['PARAM_SEPARATOR'] . $skills_user . $GLOBALS['PARAM_SEPARATOR'] . $description_user . $GLOBALS['PARAM_SEPARATOR'] . $additionalrequest_user . $GLOBALS['PARAM_SEPARATOR'] . $additionaloffer_user . $GLOBALS['PARAM_SEPARATOR'] . $banned_user;
			
			// GET LINKED IMAGES
			$table_to_check = "users";
			$query_imgs_consult = "SELECT id,size,type,url FROM images WHERE idorigin = $id_user AND tabla='$table_to_check'";
			$result_imgs_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_imgs_consult) or die("Query Error::UserConsult::Select images failed");
		
			while ($row_imgs_consult = mysqli_fetch_object($result_imgs_consult))
			{
				$line_img = $row_imgs_consult->id . $GLOBALS['PARAM_SEPARATOR'] . "users" . $GLOBALS['PARAM_SEPARATOR'] . $user_par . $GLOBALS['PARAM_SEPARATOR'] . $row_imgs_consult->size . $GLOBALS['PARAM_SEPARATOR'] . $row_imgs_consult->type . $GLOBALS['PARAM_SEPARATOR'] . $row_imgs_consult->url;				
				$output_packet = $output_packet . $GLOBALS['LINE_SEPARATOR'] . $line_img;
			}
			
			print $output_packet;
		}
		else
		{
			print "false";
		}
		
		mysqli_free_result($result_consult);
    }
	
?>
