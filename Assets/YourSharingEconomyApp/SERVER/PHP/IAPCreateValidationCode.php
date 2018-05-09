<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$code = $_GET["code"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		InsertCodeIAP($iduser, $code);
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
     //  InsertCodeIAP
     //-------------------------------------------------------------
     function InsertCodeIAP($iduser_par, $code_par)
     {
		$code_corrected = str_replace(" ","+",$code_par);
		$query_update_user = "UPDATE users SET purchasecode = '$code_corrected' WHERE id = $iduser_par";
		$result_update_user = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user) or die("Query Error::IAPCreateValidationCode::InsertCodeIAP");

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
