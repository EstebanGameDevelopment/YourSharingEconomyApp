<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';
	 
	$iduser = $_GET["id"];
	$password = $_GET["password"];
	
	$latitude_rq = $_GET["latitude"];
	$longitude_rq = $_GET["longitude"];
	$distance_rq = $_GET["distance"];
	$state_rq = $_GET["state"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		ConsultRequestsByDistance($latitude_rq, $longitude_rq, $distance_rq, $state_rq);
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
     //  Distance2Points
     //-------------------------------------------------------------
	function Distance2Points($lat1, $lon1, $lat2, $lon2, $unit) 
	{
	  $theta = $lon1 - $lon2;
	  $dist = sin(deg2rad($lat1)) * sin(deg2rad($lat2)) +  cos(deg2rad($lat1)) * cos(deg2rad($lat2)) * cos(deg2rad($theta));
	  $dist = acos($dist);
	  $dist = rad2deg($dist);
	  $miles = $dist * 60 * 1.1515;
	  $unit = strtoupper($unit);

	  if ($unit == "K") {
		return ($miles * 1.609344);
	  } else if ($unit == "N") {
		  return ($miles * 0.8684);
		} else {
			return $miles;
		  }
		  
		// echo distance(32.9697, -96.80322, 29.46786, -98.53506, "M") . " Miles<br>";
		// echo distance(32.9697, -96.80322, 29.46786, -98.53506, "K") . " Kilometers<br>";
		// echo distance(32.9697, -96.80322, 29.46786, -98.53506, "N") . " Nautical Miles<br>";
	}

     //-------------------------------------------------------------
     //  CheckInsideDesiredRange
     //-------------------------------------------------------------
     function CheckInsideDesiredRange($latitude_par, $longitude_par, $latitude_goal_par, $longitude_goal_par, $distance_par)
     {
		$finalDistance = 1;
		switch ($distance_par)
		{
			// 5 KM
			case 0:
				$finalDistance = 5;
				break;
			
			// 10 KM
			case 1:
				$finalDistance = 10;
				break;

			// 20 KM
			case 2:
				$finalDistance = 20;
				break;
				
			// 50 KM
			case 3:
				$finalDistance = 50;
				break;

			// 100 KM
			case 4:
				$finalDistance = 100;
				break;
		}
		
		$distanceFound = Distance2Points($latitude_par, $longitude_par, $latitude_goal_par, $longitude_goal_par, "K");
		
		if ($distanceFound <= $finalDistance)
		{
			return true;
		}			
	 }

     //-------------------------------------------------------------
     //  ConsultRequestsByDistance
     //-------------------------------------------------------------
     function ConsultRequestsByDistance($latitude_par, $longitude_par, $distance_par, $state_par)
     {
		$query_consult = "SELECT * FROM requests WHERE (abs(longitude - $longitude_par)<1) AND (abs(latitude - $latitude_par)<1) AND flaged = 0 ORDER BY creationdate DESC";
		$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::RequestConsultByDistance::Select latitude and longitude error");

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
			$latitude_req = $row_consult->latitude;
			$longitude_req = $row_consult->longitude;
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

			if (CheckInsideDesiredRange($latitude_req, $longitude_req, $latitude_par, $longitude_par, $distance_par))
			{
				$addNewRecordConfirmation = false;
				switch ($state_par)
				{
					// ALL
					case -1:
						$addNewRecordConfirmation = true;		
						break;
						
					// OPEN
					case 0:
						if (($provider_req == -1) && ($scorecustomer_req == -1)&& ($scoreprovider_req == -1))
						{
							$addNewRecordConfirmation = true;		
						}
						break;
						
					// CLOSED
					case 1:
						if (($provider_req != -1) && ($scorecustomer_req == -1)&& ($scoreprovider_req == -1))
						{
							$addNewRecordConfirmation = true;		
						}
						break;						
					
					// FINISHED
					case 2:
						if (($scorecustomer_req != -1)&& ($scoreprovider_req != -1))
						{
							$addNewRecordConfirmation = true;		
						}
						break;
				}
						
				if ($addNewRecordConfirmation)
				{
					$line_request = $id_req . $GLOBALS['PARAM_SEPARATOR'] . $customer_req . $GLOBALS['PARAM_SEPARATOR'] . $provider_req . $GLOBALS['PARAM_SEPARATOR'] . $title_req . $GLOBALS['PARAM_SEPARATOR'] . $price_req . $GLOBALS['PARAM_SEPARATOR'] . $deadline_req . $GLOBALS['PARAM_SEPARATOR'] . $score_req . $GLOBALS['PARAM_SEPARATOR'] . $deliverydate_req . $GLOBALS['PARAM_SEPARATOR'] . $currency_req. $GLOBALS['PARAM_SEPARATOR'] . $mapdata_req . $GLOBALS['PARAM_SEPARATOR'] . $referenceimg_req . $GLOBALS['PARAM_SEPARATOR'] . $village_req . $GLOBALS['PARAM_SEPARATOR'] . $proposal_req . $GLOBALS['PARAM_SEPARATOR'] . $feedbackcustomer_req . $GLOBALS['PARAM_SEPARATOR'] . $scorecustomer_req . $GLOBALS['PARAM_SEPARATOR'] . $feedbackprovider_req . $GLOBALS['PARAM_SEPARATOR'] . $scoreprovider_req . $GLOBALS['PARAM_SEPARATOR'] . "-1" . $GLOBALS['PARAM_SEPARATOR'] . $reported_req . $GLOBALS['PARAM_SEPARATOR'] . $flaged_req;
				
					$package = $package . $GLOBALS['LINE_SEPARATOR'] . $line_request;
				}
			}
		}
		
		print $package;
		
		mysqli_free_result($result_consult);
    }
	
?>
