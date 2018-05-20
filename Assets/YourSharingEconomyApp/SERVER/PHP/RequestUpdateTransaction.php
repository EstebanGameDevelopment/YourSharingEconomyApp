<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$requestid = $_GET["request"];
	$transaction = $_GET["transaction"];	

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		UpdateTransactionID($iduser, $requestid, $transaction);
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
     //  UpdateTransactionID
     //-------------------------------------------------------------
     function UpdateTransactionID($iduser_par, $requestid_par, $transaction_par)
     {		 
		// UPDATE QUERY
		$query_update_request = "UPDATE requests SET transactionid='$transaction_par' WHERE id = $requestid_par AND customer = $iduser_par";
		$result_update_request = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_request) or die("Query Error::UpdateTransactionID::Update request failed");

		// Free resultset
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
		{
			print "true";
		}
		else
		{
			print "false";
		}
    }
	
?>
