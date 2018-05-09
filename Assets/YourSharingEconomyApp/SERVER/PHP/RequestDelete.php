<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$request = $_GET["request"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		DeleteRequest($request);
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
     //  DeleteRequest
     //-------------------------------------------------------------
     function DeleteRequest($request_par)
     {
		// CHECK IF THE REQUEST HAS BEEN REPORTED
		$query_report_consult = "SELECT customer,provider,reported FROM requests WHERE id = $request_par";
		$result_report_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_report_consult) or die("Query Error::RequestDelete::Select request failed");

		$run_delete = true;
		$customer_request = -1;
		$provider_request = -1;
		if ($row_report_consult = mysqli_fetch_object($result_report_consult))
		{
			$customer_request = $row_report_consult->customer; 
			$provider_request = $row_report_consult->provider; 
			$users_who_reported = $row_report_consult->reported;
			if (strlen($users_who_reported)>0)
			{
				$run_delete = false;
			}
		}

		// RUN DELETE IF NOT REPORTED
		if ($run_delete)
		{
			$query_delete = "DELETE FROM requests WHERE id = $request_par";
			$result_delete = mysqli_query($GLOBALS['LINK_DATABASE'],$query_delete) or die("Query Error::RequestDelete::Delete request failed");

			if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
			{
				$requests_name = "requests";
				$query_img_delete = "DELETE FROM images WHERE idorigin = $request_par AND tabla='$requests_name'";
				$result_img_delete = mysqli_query($GLOBALS['LINK_DATABASE'],$query_img_delete) or die("Query Error::RequestDelete::Delete images failed");

				$query_prop_delete = "DELETE FROM proposals WHERE request = $request_par";
				$result_prop_delete = mysqli_query($GLOBALS['LINK_DATABASE'],$query_prop_delete) or die("Query Error::RequestDelete::Delete proposals failed");
				
				// APPLY PENALTY
				if (($provider_request != -1) && ($customer_request!=-1))
				{
					$query_update_user_score_provider = "UPDATE users SET votesuser = votesuser + 1 WHERE id = $customer_request";
					$result_update_user_score_provider = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user_score_provider) or die("Query Error::RequestDelete::Update users failed");
				}
					
				print "true" . $GLOBALS['PARAM_SEPARATOR'] . $request_par;
			}
			else
			{
				print "false";
			}
		}
		else
		{
			print "false" . $GLOBALS['PARAM_SEPARATOR'] . "reported_request_no_delete_available";
		}		
    }
	
?>
