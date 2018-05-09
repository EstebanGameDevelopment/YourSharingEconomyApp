<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$iduser = $_POST["id"];
	$password = $_POST["password"];
	$requestid = $_POST["request"];
	
	$scorecustomer = $_POST["scorecustomer"];
	$feedbackcustomer = $_POST["feedbackcustomer"];
	$scoreprovider = $_POST["scoreprovider"];
	$feedbackprovider = $_POST["feedbackprovider"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{	
		UpdateRequestScoreAndFeedback($requestid, $scorecustomer, $feedbackcustomer, $scoreprovider, $feedbackprovider);
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
     //  UpdateRequestAsFinished
     //-------------------------------------------------------------
     function UpdateRequestScoreAndFeedback($requestid_par, $scorecustomer_par, $feedbackcustomer_par, $scoreprovider_par, $feedbackprovider_par)
     {
		$feedbackcustomer_res = SpecialCharacters($feedbackcustomer_par);
		$feedbackprovider_res = SpecialCharacters($feedbackprovider_par);
		
		$query_consult = "SELECT customer,provider,scorecustomer,scoreprovider FROM requests WHERE id = $requestid_par";
		$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::RequestUpdateScore::Select request failed");

		if ($row_consult = mysqli_fetch_object($result_consult))
		{
			$customer_rq = $row_consult->customer;
			$provider_rq = $row_consult->provider;
			
			$scorecustomer_rq = $row_consult->scorecustomer;
			$scoreprovider_rq = $row_consult->scoreprovider;
			
			// UPDATE SCORE CUSTOMER TO provider
			if (($scorecustomer_rq==-1) && ($scorecustomer_par != -1))
			{
				$query_update_user_score_provider = "UPDATE users SET votesprovider = votesprovider + 1, scoreprovider = scoreprovider + $scorecustomer_par WHERE id = $provider_rq";
				$result_update_user_score_provider = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user_score_provider) or die("Query Error::RequestUpdateScore::Update users failed");
			}
				
			// UPDATE SCORE provider TO CUSTOMER
			if (($scoreprovider_rq==-1) && ($scoreprovider_par != -1))
			{
				$query_update_user_score_customer = "UPDATE users SET votesuser = votesuser + 1, scoreuser = scoreuser + $scoreprovider_par WHERE id = $customer_rq";
				$result_update_user_score_customer = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user_score_customer) or die("Query Error::RequestUpdateScore::Update users part 2 failed");
			}
			
			// UPDATE QUERY
			$query_update_request = "UPDATE requests SET scorecustomer=$scorecustomer_par, scoreprovider=$scoreprovider_par, feedbackcustomer='$feedbackcustomer_res', feedbackprovider='$feedbackprovider_res' WHERE id = $requestid_par";
			$result_update_request = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_request) or die("Query Error::RequestUpdateScore::Update request failed");

			// Free resultset
			if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
			{
				print "true" . $GLOBALS['PARAM_SEPARATOR'] . $requestid_par;
			}
			else
			{
				print "false";
			}
		}		
	 }

	
?>
