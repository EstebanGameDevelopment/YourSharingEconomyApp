<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$rent = $_GET["rent"];
	$code = $_GET["code"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		RentTimeProvider($iduser, $rent, $code);
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
     //  RentTimeProvider
     //-------------------------------------------------------------
     function RentTimeProvider($iduser_par, $rent_par, $code_par)
     {
		$current_time_start_rent = GetCurrentTimestamp();
		$rentdays_user = 0;
		switch ($rent_par)
		{
			case 1:
				$rentdays_user = 30;
				break;
				
			case 2:
				$rentdays_user = 90;
				break;

			case 3:
				$rentdays_user = 180;
				break;

			case 4:
				$rentdays_user = 365;
				break;
		}

		$code_corrected = str_replace(" ","+",$code_par);		
		$query_update_user = "UPDATE users SET rentstart = $current_time_start_rent, rentdays = $rentdays_user, purchasecode='' WHERE id = $iduser_par AND purchasecode='$code_corrected'";
		$result_update_user = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user) or die("Query Error::IAPRentTimeProvider::RentTimeProvider");

		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
		{
			print "true" . $GLOBALS['PARAM_SEPARATOR'] . $code_corrected;
		}
		else
		{
			print "false";
		}
    }
	
?>
