<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$iduser = $_POST["id"];
	$currpass = $_POST["currpass"];
	$password = $_POST["password"];
	$email = $_POST["email"];
	$nameuser = $_POST["name"];
	$village = $_POST["village"];
	$mapdata = $_POST["mapdata"];
	$skills = $_POST["skills"];
	$description = $_POST["description"];
	$publickey = $_POST["publickey"];

    // ++ LOGIN WITH email ++
	UpdateProfile($iduser, $currpass, $password, $email, $nameuser, $village, $mapdata, $skills, $description, $publickey);

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
     //  UpdateProfile
     //-------------------------------------------------------------
     function UpdateProfile($iduser_par, $currpassword_par, $newpassword_par, $email_par, $nameuser_par, $village_par, $mapdata_par, $skills_par, $description_par, $publickey_par)
     {
		$new_email_encrypted = EncryptText($email_par);
		$currpassword_encrypted = EncryptText($currpassword_par);
		$newpassword_encrypted = EncryptText($newpassword_par);
		 
		// Performing SQL Consult
		$query_user = "SELECT id FROM users WHERE id = $iduser_par AND password = '$currpassword_encrypted'";
		$result_user = mysqli_query($GLOBALS['LINK_DATABASE'],$query_user) or die("Query Error::UserUpdateProfile::Select users failed");
				
		if ($row_user = mysqli_fetch_object($result_user))
		{
			$coordinates_map = explode(",", $mapdata_par);
			if (sizeof($coordinates_map) == 2)
			{
				$coordinates_map_latitude = floatval($coordinates_map[0]);
				$coordinates_map_longitude = floatval($coordinates_map[1]);
			}
			else
			{
				$coordinates_map_latitude = -1;
				$coordinates_map_longitude = -1;
			}

			$nameuser_par_f = SpecialCharacters($nameuser_par);
			$description_par_f = SpecialCharacters($description_par);
			$village_par_f = SpecialCharacters($village_par);
			
			// UPDATE ENERGY
			$query_update_user = "UPDATE users SET password = '$newpassword_encrypted', name = '$nameuser_par_f', email='$new_email_encrypted', village = '$village_par_f', mapdata = '$mapdata_par', latitude=$coordinates_map_latitude, longitude=$coordinates_map_longitude, skills = '$skills_par', description = '$description_par_f', publickey = '$publickey_par' WHERE id = $iduser_par";
			$result_update_user = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user) or die("Query Error::UserUpdateProfile::Update users failed");
			
			print "true";
		}
		else
		{
			print "false";
		}
	 
		// Free resultset
		mysqli_free_result($result_user);
    }
	
?>
