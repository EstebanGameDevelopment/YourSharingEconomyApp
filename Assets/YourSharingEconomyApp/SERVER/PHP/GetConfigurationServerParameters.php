<?php

	include 'ConfigurationYourSharingEconomyApp.php';

	$message = "";
	$message = $message . "true" . $GLOBALS['PARAM_SEPARATOR'];
	$message = $message . $GLOBALS['TOTAL_REPORT_REQUEST_TO_WARN_USERS'] . $GLOBALS['PARAM_SEPARATOR'];
	$message = $message . $GLOBALS['HOURS_TO_WAIT_FOR_NEW_PROPOSAL'] . $GLOBALS['PARAM_SEPARATOR'];
	$message = $message . $GLOBALS['FREE_REQUESTS_AVAILABLE_TO_CONSUMERS'] . $GLOBALS['PARAM_SEPARATOR'];
	$message = $message . $GLOBALS['TOTAL_NUMBER_IMAGES_AS_REFERENCE'] . $GLOBALS['PARAM_SEPARATOR'];
	$message = $message . $GLOBALS['TOTAL_NUMBER_IMAGES_AS_FINISHED'] . $GLOBALS['PARAM_SEPARATOR'];
	$message = $message . $GLOBALS['TOTAL_NUMBER_IMAGES_PROVIDER_EXPERIENCE'] . $GLOBALS['PARAM_SEPARATOR'];	
	$message = $message . $GLOBALS['SIZE_HEIGHT_ALLOWED_IMAGES']. $GLOBALS['PARAM_SEPARATOR'];	
	$message = $message . $GLOBALS['PROVIDER_SKILLS'];
	
	print  $message;	
?>