<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$languageuser = $_POST["language"];
	$iduser = $_POST["id"];
	$password = $_POST["password"];

	$requestid = $_POST["request"];
	$customer = $_POST["customer"];
	$provider = $_POST["provider"];
	$title = $_POST["title"];
	$description = $_POST["description"];
	$images = $_POST["images"];
	$referenceimg = $_POST["referenceimg"];
	$village = $_POST["village"];
	$mapdata = $_POST["mapdata"];
	$latitude = $_POST["latitude"];
	$longitude = $_POST["longitude"];
	$price = $_POST["price"];
	$currency = $_POST["currency"];
	$distance = $_POST["distance"];
	$flags = $_POST["flags"];
	$notifications = $_POST["notifications"];
	$creationdate = $_POST["creationdate"];
	$deadline = $_POST["deadline"];
	$feedbackcustomer = $_POST["feedbackcustomer"];
	$scorecustomer = $_POST["scorecustomer"];
	$feedbackprovider = $_POST["feedbackprovider"];
	$scoreprovider = $_POST["scoreprovider"];
	
	$creationdate_in_server = time();
	
    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{		
		if ($requestid == -1)
		{
			CreateRequest($languageuser, $email_db_user, $customer, $provider,	$title,	$description, $images, $referenceimg, $village, $mapdata, $latitude, $longitude, $price, $currency, $distance, $flags, $notifications,	$creationdate_in_server, $deadline, $feedbackcustomer, $scorecustomer, $feedbackprovider, $scoreprovider);
		}
		else
		{
			UpdateRequest($languageuser, $email_db_user, $requestid, $customer, $provider,	$title,	$description, $images, $referenceimg, $village, $mapdata, $latitude, $longitude, $price, $currency, $distance, $flags, $notifications,	$creationdate_in_server, $deadline, $feedbackcustomer, $scorecustomer, $feedbackprovider, $scoreprovider);
		}
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
     //  CheckExistingRequestsForUser
     //-------------------------------------------------------------	 
	 function CheckExistingRequestsForUser($iduser_par)
	 {
		// CHECK MAXIMUM NUMBER OF FREE REQUEST
		$query_req_consult = "SELECT provider,deliverydate FROM requests WHERE customer = $iduser_par AND (provider = -1 OR deliverydate = -1) ORDER BY creationdate DESC";
		$result_req_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_req_consult) or die("Query Error::RequestCreate::CheckExistingRequestsForUser::Select requests failed");

		$counter_records = 0;
		while ($row_req_consult = mysqli_fetch_object($result_req_consult))
		{
			$counter_records = $counter_records + 1;
		}
		
		if ($counter_records > $GLOBALS['FREE_REQUESTS_AVAILABLE_TO_CONSUMERS'])
		{
			// CHECK IF PREMIUM ADDITIONAL REQUEST
			$query_usr_consult = "SELECT additionalrequest FROM users WHERE id = $iduser_par";
			$result_usr_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_usr_consult) or die("Query Error::RequestCreate::CheckExistingRequestsForUser::Select users failed");

			if ($row_user = mysqli_fetch_object($result_usr_consult))
			{
				$additionalrequest_user = $row_user->additionalrequest;
				if ($additionalrequest_user > 0)
				{
					return true;
				}
			}
			
			return false;
		}
		else
		{
			return true;
		}	
	 }
	 
     //-------------------------------------------------------------
     //  CreateRequest
     //-------------------------------------------------------------
     function CreateRequest($languageuser_par, $email_par, $customer_par, $provider_par,	$title_par,	$description_par, $images_par, $referenceimg_par, $village_par, $mapdata_par, $latitude_par, $longitude_par, $price_par, $currency_par, $distance_par, $flags_par,	$notifications_par, $creationdate_par, $deadline_par, $feedbackcustomer_par, $scorecustomer_par, $feedbackprovider_par, $scoreprovider_par)
     {
		 if (CheckExistingRequestsForUser($customer_par))
		 {
			$query_consult = "SELECT max(id) as maximumId FROM requests";
			$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::RequestCreate::CreateRequest::Get max request id failed");
			$row_consult = mysqli_fetch_object($result_consult);
			$maxIdentifier = $row_consult->maximumId;
			mysqli_free_result($result_consult);

			$request_id_new = $maxIdentifier + 1;

			$title_par_f = SpecialCharacters($title_par);
			$description_par_f = SpecialCharacters($description_par);
			$village_par_f = SpecialCharacters($village_par);
			$feedbackcustomer_par_f = SpecialCharacters($feedbackcustomer_par);
			$feedbackprovider_par_f = SpecialCharacters($feedbackprovider_par);

			$query_insert = "INSERT INTO requests VALUES ($request_id_new, $customer_par, $provider_par,	'$title_par_f', '$description_par_f', $images_par, $referenceimg_par, '$village_par_f', '$mapdata_par', $latitude_par, $longitude_par, $price_par, '$currency_par', $distance_par, '$flags_par', $notifications_par, $creationdate_par, $deadline_par, -1, -1, 0, -1, '$feedbackcustomer_par_f', $scorecustomer_par, '$feedbackprovider_par_f', $scoreprovider_par, '', 0, 0, '', '', '')";
			$result_insert = mysqli_query($GLOBALS['LINK_DATABASE'],$query_insert) or die("Query Error::RequestCreate::CreateRequest::Insert failed");
			
			// Free resultset
			if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
			{
				// MailPassword($language_par, $email_par, $password_par, $user_id_new, $random_code_validate);
				
				// UPDATE PREMIUM REQUEST
				$query_update_user = "UPDATE users SET additionalrequest = 0 WHERE id = $customer_par";
				$result_update_user = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user) or die("Query Error::RequestCreate::CreateRequest::Update users failed");
				
				print "true" . $GLOBALS['PARAM_SEPARATOR'] . $request_id_new;
			}
			else
			{
				print "false" . $GLOBALS['PARAM_SEPARATOR'] . "CREATION";
			}
		 }
		 else
		 {
			 print "false" . $GLOBALS['PARAM_SEPARATOR'] . "CREATION";
		 }
    }
	
	 //-------------------------------------------------------------
     //  UpdateRequest
     //-------------------------------------------------------------
     function UpdateRequest($languageuser_par, $email_par, $requestid_par, $customer_par, $provider_par,	$title_par,	$description_par, $images_par, $referenceimg_par, $village_par, $mapdata_par, $latitude_par, $longitude_par, $price_par, $currency_par, $distance_par, $flags_par,	$notifications_par, $creationdate_par, $deadline_par, $feedbackcustomer_par, $scorecustomer_par, $feedbackprovider_par, $scoreprovider_par)
     {
		$title_par_f = SpecialCharacters($title_par);
		$description_par_f = SpecialCharacters($description_par);
		$village_par_f = SpecialCharacters($village_par);
		$feedbackcustomer_par_f = SpecialCharacters($feedbackcustomer_par);
		$feedbackprovider_par_f = SpecialCharacters($feedbackprovider_par);
		 
		$query_update_request = "UPDATE requests SET customer=$customer_par, provider=$provider_par,	title='$title_par_f',	description='$description_par_f', images=$images_par, village='$village_par_f', mapdata='$mapdata_par', latitude=$latitude_par, longitude=$longitude_par, price=$price_par, currency='$currency_par', distance=$distance_par, flags='$flags_par', notifications=$notifications_par, creationdate=$creationdate_par, deadline=$deadline_par, referenceimg=$referenceimg_par, feedbackcustomer='$feedbackcustomer_par_f', scorecustomer=$scorecustomer_par, feedbackprovider='$feedbackprovider_par_f', scoreprovider=$scoreprovider_par WHERE id = $requestid_par";
		$result_update_request = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_request) or die("Query Error::RequestCreate::UpdateRequest::Update requests failed");

		// Free resultset
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
		{
			// MailPassword($language_par, $email_par, $password_par, $user_id_new, $random_code_validate);
			print "true" . $GLOBALS['PARAM_SEPARATOR'] . $requestid_par;
		}
		else
		{
			print "false" . $GLOBALS['PARAM_SEPARATOR'] . "UPDATE" . $GLOBALS['PARAM_SEPARATOR'] . $requestid_par;
		}
	 }

	
?>
