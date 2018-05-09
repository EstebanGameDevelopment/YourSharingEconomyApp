<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$requestid = $_GET["request"];
	$imgreference = $_GET["imgreference"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{	
		UpdateImageReference($requestid, $imgreference);
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
     //  UpdateImageReference
     //-------------------------------------------------------------
     function UpdateImageReference($requestid_par, $imgreference_par)
     {
		$query_update_request = "UPDATE requests SET referenceimg=$imgreference_par WHERE id = $requestid_par";
		$result_update_request = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_request) or die("Query Error::RequestUpdateImageReference::Update requests failed");

		// Free resultset
		print "true" . $GLOBALS['PARAM_SEPARATOR'] . $requestid_par;
	 }

	
?>
