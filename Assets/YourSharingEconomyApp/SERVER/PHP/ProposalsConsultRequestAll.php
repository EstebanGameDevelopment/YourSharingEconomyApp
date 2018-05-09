<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';
	 
	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$request = $_GET["request"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		ConsultRequestAllProposals($request);
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
     //  ConsultRequestAllProposals
     //-------------------------------------------------------------
     function ConsultRequestAllProposals($request_par)
     {
		$query_prop_consult = "SELECT * FROM proposals WHERE request = $request_par ORDER BY created DESC";
		$result_prop_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_prop_consult) or die("Query Error::PropsalsConsultRequestAll::Failed to consult proposals");
	
		$output_packet = "";
		while ($row_prop_consult = mysqli_fetch_object($result_prop_consult))		
		{
			$id_prop = $row_prop_consult->id;
			$user_prop = $row_prop_consult->user;
			$type_prop = $row_prop_consult->type;
			$title_prop = $row_prop_consult->title;
			$description_prop = $row_prop_consult->description;
			$price_prop = $row_prop_consult->price;
			$deadline_prop = $row_prop_consult->deadline;
			$accepted_prop = $row_prop_consult->accepted;
			$active_prop = $row_prop_consult->active;
			$created_prop = $row_prop_consult->created;
			$reported_prop = $row_prop_consult->reported;
			$confirmedreported_prop = $row_prop_consult->confirmedreported;
			
			$line_prop = $id_prop . $GLOBALS['PARAM_SEPARATOR'] .  $user_prop . $GLOBALS['PARAM_SEPARATOR'] . $request_par . $GLOBALS['PARAM_SEPARATOR'] . $type_prop . $GLOBALS['PARAM_SEPARATOR'] . $title_prop . $GLOBALS['PARAM_SEPARATOR'] . $description_prop . $GLOBALS['PARAM_SEPARATOR'] . $price_prop . $GLOBALS['PARAM_SEPARATOR'] . $deadline_prop . $GLOBALS['PARAM_SEPARATOR'] . $accepted_prop . $GLOBALS['PARAM_SEPARATOR'] . $active_prop . $GLOBALS['PARAM_SEPARATOR'] . $created_prop . $GLOBALS['PARAM_SEPARATOR'] . $reported_prop . $GLOBALS['PARAM_SEPARATOR'] . $confirmedreported_prop;
			
			$output_packet = $output_packet . $GLOBALS['LINE_SEPARATOR'] . $line_prop;
		}
		
		print $output_packet;		
		
		mysqli_free_result($result_prop_consult);
    }
	
?>
