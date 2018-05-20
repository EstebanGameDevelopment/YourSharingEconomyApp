<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';
    
	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$provider = $_GET["provider"];
	$all_request = $_GET["all"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		ConsultRequestsByProvider($provider, $all_request);
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
     //  ConsultRequestsByProvider
     //-------------------------------------------------------------
     function ConsultRequestsByProvider($provider_par, $all_request_par)
     {
		$query_consult = "SELECT * FROM requests WHERE provider = $provider_par ORDER BY creationdate DESC";
		$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::ConsultRequestsByProvider::Select requests failed");

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

			$line_request = $id_req . $GLOBALS['PARAM_SEPARATOR'] . $customer_req . $GLOBALS['PARAM_SEPARATOR'] . $provider_req . $GLOBALS['PARAM_SEPARATOR'] . $title_req . $GLOBALS['PARAM_SEPARATOR'] . $price_req . $GLOBALS['PARAM_SEPARATOR'] . $deadline_req . $GLOBALS['PARAM_SEPARATOR'] . $score_req . $GLOBALS['PARAM_SEPARATOR'] . $deliverydate_req . $GLOBALS['PARAM_SEPARATOR'] . $currency_req . $GLOBALS['PARAM_SEPARATOR'] . $mapdata_req . $GLOBALS['PARAM_SEPARATOR'] . $referenceimg_req . $GLOBALS['PARAM_SEPARATOR'] . $village_req . $GLOBALS['PARAM_SEPARATOR'] . $proposal_req . $GLOBALS['PARAM_SEPARATOR'] . $feedbackcustomer_req . $GLOBALS['PARAM_SEPARATOR'] . $scorecustomer_req . $GLOBALS['PARAM_SEPARATOR'] . $feedbackprovider_req . $GLOBALS['PARAM_SEPARATOR'] . $scoreprovider_req . $GLOBALS['PARAM_SEPARATOR'] . $provider_par . $GLOBALS['PARAM_SEPARATOR'] . $reported_req . $GLOBALS['PARAM_SEPARATOR'] . $flaged_req . $GLOBALS['PARAM_SEPARATOR'] . $transactionid_req;
			
			$package = $package . $GLOBALS['LINE_SEPARATOR'] . $line_request;
		}

		mysqli_free_result($result_consult);
		
		// FOLLOWING REQUESTS
		$query_prop_consult = "SELECT request FROM proposals WHERE user = $provider_par AND type <> 2";
		if ($all_request_par == 1)
		{
			$query_prop_consult = "SELECT request FROM proposals WHERE user = $provider_par";	
		}
		$result_prop_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_prop_consult) or die("Query Error::ConsultRequestsByProvider::Select proposals failed");
	
		while ($row_prop_consult = mysqli_fetch_object($result_prop_consult))		
		{
			$request_prop = $row_prop_consult->request;

			$query_rq_consult = "SELECT * FROM requests WHERE id = $request_prop AND provider = -1 AND customer <> $provider_par ORDER BY creationdate DESC";
			$result_rq_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_rq_consult) or die("Query Error::ConsultRequestsByProvider::Select requests part 2 failed");

			while ($row_rq_consult = mysqli_fetch_object($result_rq_consult))
			{
				$id_req = $row_rq_consult->id;
				$customer_req = $row_rq_consult->customer;
				$provider_req = $row_rq_consult->provider;
				$title_req = $row_rq_consult->title;
				$price_req = $row_rq_consult->price;
				$currency_req = $row_rq_consult->currency;
				$deadline_req = $row_rq_consult->deadline;
				$score_req = $row_rq_consult->score;
				$deliverydate_req = $row_rq_consult->deliverydate;
				$mapdata_req = $row_rq_consult->mapdata;
				$referenceimg_req = $row_rq_consult->referenceimg;
				$village_req = $row_rq_consult->village;
				$proposal_req = $row_rq_consult->proposal;
				$feedbackcustomer_req = $row_rq_consult->feedbackcustomer;
				$scorecustomer_req = $row_rq_consult->scorecustomer;
				$feedbackprovider_req = $row_rq_consult->feedbackprovider;
				$scoreprovider_req = $row_rq_consult->scoreprovider;
				$reportedprovider_req = $row_rq_consult->reported;
				$flagedprovider_req = $row_rq_consult->flaged;
				$transactionidprovider_req = $row_rq_consult->transactionid;

				$line_request = $id_req . $GLOBALS['PARAM_SEPARATOR'] . $customer_req . $GLOBALS['PARAM_SEPARATOR'] . $provider_req . $GLOBALS['PARAM_SEPARATOR'] . $title_req . $GLOBALS['PARAM_SEPARATOR'] . $price_req . $GLOBALS['PARAM_SEPARATOR'] . $deadline_req . $GLOBALS['PARAM_SEPARATOR'] . $score_req . $GLOBALS['PARAM_SEPARATOR'] . $deliverydate_req . $GLOBALS['PARAM_SEPARATOR'] . $currency_req . $GLOBALS['PARAM_SEPARATOR'] . $mapdata_req . $GLOBALS['PARAM_SEPARATOR'] . $referenceimg_req . $GLOBALS['PARAM_SEPARATOR'] . $village_req . $GLOBALS['PARAM_SEPARATOR'] . $proposal_req . $GLOBALS['PARAM_SEPARATOR'] . $feedbackcustomer_req . $GLOBALS['PARAM_SEPARATOR'] . $scorecustomer_req . $GLOBALS['PARAM_SEPARATOR'] . $feedbackprovider_req . $GLOBALS['PARAM_SEPARATOR'] . $scoreprovider_req . $GLOBALS['PARAM_SEPARATOR'] . $provider_par. $GLOBALS['PARAM_SEPARATOR'] . $reportedprovider_req . $GLOBALS['PARAM_SEPARATOR'] . $flagedprovider_req . $GLOBALS['PARAM_SEPARATOR'] . $transactionidprovider_req;
				
				$package = $package . $GLOBALS['LINE_SEPARATOR'] . $line_request;
			}			
			
			mysqli_free_result($result_rq_consult);
		}
		
		mysqli_free_result($result_prop_consult);
		
		print $package;
		
		
    }
	
?>
