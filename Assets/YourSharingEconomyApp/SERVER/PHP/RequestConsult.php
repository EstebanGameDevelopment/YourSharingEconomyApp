<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';
	 
	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$request = $_GET["request"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		ConsultRequest($request);
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
     function ConsultRequest($request_par)
     {
		$query_consult = "SELECT * FROM requests WHERE id = $request_par";
		$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::RequestConsult::Select requests");

		$package = "";
		if ($row_consult = mysqli_fetch_object($result_consult))
		{
			$customer_req = $row_consult->customer;
			$provider_req = $row_consult->provider;
			$title_req = $row_consult->title;
			$description_req = $row_consult->description;
			$images_req = $row_consult->images;
			$referenceimg_req = $row_consult->referenceimg;
			$village_req = $row_consult->village;
			$mapdata_req = $row_consult->mapdata;
			$latitude_req = $row_consult->latitude;
			$longitude_req = $row_consult->longitude;
			$price_req = $row_consult->price;
			$currency_req = $row_consult->currency;
			$distance_req = $row_consult->distance;
			$flags_req = $row_consult->flags;
			$notifications_req = $row_consult->notifications;
			$creationdate_req = $row_consult->creationdate;
			$deadline_req = $row_consult->deadline;
			$score_req = $row_consult->score;
			$deliverydate_req = $row_consult->deliverydate;
			$workdays_req = $row_consult->workdays;
			$proposal_req = $row_consult->proposal;
			$feedbackcustomer_req = $row_consult->feedbackcustomer;
			$scorecustomer_req = $row_consult->scorecustomer;
			$feedbackprovider_req = $row_consult->feedbackprovider;
			$scoreprovider_req = $row_consult->scoreprovider;
			$reported_req = $row_consult->reported;
			$flaged_req = $row_consult->flaged;
			
			$output_packet =  $request_par . $GLOBALS['PARAM_SEPARATOR'] . $customer_req . $GLOBALS['PARAM_SEPARATOR'] . $provider_req . $GLOBALS['PARAM_SEPARATOR'] . $title_req . $GLOBALS['PARAM_SEPARATOR'] . $description_req . $GLOBALS['PARAM_SEPARATOR'] . $images_req . $GLOBALS['PARAM_SEPARATOR'] . $referenceimg_req  . $GLOBALS['PARAM_SEPARATOR'] . $village_req . $GLOBALS['PARAM_SEPARATOR'] . $mapdata_req . $GLOBALS['PARAM_SEPARATOR'] . $latitude_req . $GLOBALS['PARAM_SEPARATOR'] . $longitude_req . $GLOBALS['PARAM_SEPARATOR'] . $price_req . $GLOBALS['PARAM_SEPARATOR'] . $currency_req . $GLOBALS['PARAM_SEPARATOR'] . $distance_req . $GLOBALS['PARAM_SEPARATOR'] . $flags_req . $GLOBALS['PARAM_SEPARATOR'] . $notifications_req . $GLOBALS['PARAM_SEPARATOR'] . $creationdate_req . $GLOBALS['PARAM_SEPARATOR'] . $deadline_req . $GLOBALS['PARAM_SEPARATOR'] . $score_req . $GLOBALS['PARAM_SEPARATOR'] . $deliverydate_req . $GLOBALS['PARAM_SEPARATOR'] . $workdays_req . $GLOBALS['PARAM_SEPARATOR'] . $proposal_req . $GLOBALS['PARAM_SEPARATOR'] . $feedbackcustomer_req . $GLOBALS['PARAM_SEPARATOR'] . $scorecustomer_req . $GLOBALS['PARAM_SEPARATOR'] . $feedbackprovider_req . $GLOBALS['PARAM_SEPARATOR'] . $scoreprovider_req . $GLOBALS['PARAM_SEPARATOR'] . $reported_req . $GLOBALS['PARAM_SEPARATOR'] . $flaged_req;
			
			// GET LINKED IMAGES
			$table_to_check = "requests";
			$query_imgs_consult = "SELECT id,size,type,url FROM images WHERE idorigin = $request_par AND tabla='$table_to_check'";
			$result_imgs_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_imgs_consult) or die("Query Error::RequestConsult::Select images");
		
			while ($row_imgs_consult = mysqli_fetch_object($result_imgs_consult))		
			{
				$line_img = $row_imgs_consult->id . $GLOBALS['PARAM_SEPARATOR'] . "requests" . $GLOBALS['PARAM_SEPARATOR'] . $request_par . $GLOBALS['PARAM_SEPARATOR'] . $row_imgs_consult->size . $GLOBALS['PARAM_SEPARATOR'] . $row_imgs_consult->type . $GLOBALS['PARAM_SEPARATOR'] . $row_imgs_consult->url;
				
				$output_packet = $output_packet . $GLOBALS['LINE_SEPARATOR'] . $line_img;
			}
			
			print $output_packet;
			
			mysqli_free_result($result_imgs_consult);
		}
		else
		{
			print $GLOBALS['PARAM_SEPARATOR'];
		}
		
		mysqli_free_result($result_consult);
    }
	
?>
