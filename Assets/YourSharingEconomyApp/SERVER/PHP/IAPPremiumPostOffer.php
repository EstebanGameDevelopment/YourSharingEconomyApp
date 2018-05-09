<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$code = $_GET["code"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		PremiumPostOffer($iduser, $code);
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
     //  PremiumPostOffer
     //-------------------------------------------------------------
     function PremiumPostOffer($iduser_par, $code_par)
     {
		$code_corrected = str_replace(" ","+",$code_par);		
		$query_update_user = "UPDATE users SET additionaloffer = 1, purchasecode='' WHERE id = $iduser_par AND purchasecode='$code_corrected'";
		$result_update_user = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user) or die("Query Error::IAPPremiumPostOffer::PremiumPostOffer");

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
