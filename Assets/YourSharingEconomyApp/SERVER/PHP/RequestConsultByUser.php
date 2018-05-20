<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$user_to_consult = $_GET["user"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		ConsultRequestsByUser($user_to_consult);		
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
     //  ConsultRequestsByUser
     //-------------------------------------------------------------
     function ConsultRequestsByUser($user_to_consult_par)
     {
		$query_consult = "SELECT * FROM requests WHERE customer = $user_to_consult_par ORDER BY creationdate DESC";
		$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::RequestConsutlByUser::Select requests failed");

		$package = "";
		while ($row_consult = mysqli_fetch_object($result_consult))
		{
			$id_req = $row_consult->id;
			$customer_req = $row_consult->customer;
			$provider_req = $row_consult->provider;
			$title_req = $row_consult->title;
			$price_req = $row_consult->price;
			$currency_req = $row_consult->currency;
			$deadline_req = $row_consult->deadline;
			$score_req = $row_consult->score;
			$deliverydate_req = $row_consult->deliverydate;
			$mapdata_req = $row_consult->mapdata;
			$referenceimg_req = $row_consult->referenceimg;
			$village_req = $row_consult->village;
			$proposal_req = $row_consult->proposal;
			$feedbackcustomer_req = $row_consult->feedbackcustomer;
			$scorecustomer_req = $row_consult->scorecustomer;
			$feedbackprovider_req = $row_consult->feedbackprovider;
			$scoreprovider_req = $row_consult->scoreprovider;
			$reported_req = $row_consult->reported;
			$flaged_req = $row_consult->flaged;
			$transactionid_req = $row_consult->transactionid;
			
			$line_request = $id_req . $GLOBALS['PARAM_SEPARATOR'] . $customer_req . $GLOBALS['PARAM_SEPARATOR'] . $provider_req . $GLOBALS['PARAM_SEPARATOR'] . $title_req . $GLOBALS['PARAM_SEPARATOR'] . $price_req . $GLOBALS['PARAM_SEPARATOR'] . $deadline_req . $GLOBALS['PARAM_SEPARATOR'] . $score_req . $GLOBALS['PARAM_SEPARATOR'] . $deliverydate_req . $GLOBALS['PARAM_SEPARATOR'] . $currency_req . $GLOBALS['PARAM_SEPARATOR'] . $mapdata_req . $GLOBALS['PARAM_SEPARATOR'] . $referenceimg_req . $GLOBALS['PARAM_SEPARATOR'] . $village_req . $GLOBALS['PARAM_SEPARATOR'] . $proposal_req . $GLOBALS['PARAM_SEPARATOR'] . $feedbackcustomer_req . $GLOBALS['PARAM_SEPARATOR'] . $scorecustomer_req . $GLOBALS['PARAM_SEPARATOR'] . $feedbackprovider_req . $GLOBALS['PARAM_SEPARATOR'] . $scoreprovider_req . $GLOBALS['PARAM_SEPARATOR'] . $user_to_consult_par . $GLOBALS['PARAM_SEPARATOR'] . $reported_req . $GLOBALS['PARAM_SEPARATOR'] . $flaged_req . $GLOBALS['PARAM_SEPARATOR'] . $transactionid_req;
			
			$package = $package . $GLOBALS['LINE_SEPARATOR'] . $line_request;
		}
		
		print $package;
		
		mysqli_free_result($result_consult);
    }
	
?>
