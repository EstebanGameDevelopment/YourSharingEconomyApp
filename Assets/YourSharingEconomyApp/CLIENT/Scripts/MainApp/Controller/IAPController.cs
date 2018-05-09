using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine.Purchasing;

/******************************************
 * 
 * IAPController
 * 
 * It manages all the IAP with the different systems of payment.
 * 
 * @author Esteban Gallardo
 */
public class IAPController : MonoBehaviour, IStoreListener
{
    // ----------------------------------------------
    // PRODUCTS
    // ----------------------------------------------
    // ANDROID
    public const string IAP_ENERGY_PACK_BASE_NAME   = "yoursharingeconomyapp.rent.with.energy.";        // DEFINE YOUR OWN IAPs
    public const string IAP_ENERGY_PACK_1           = "yoursharingeconomyapp.rent.with.energy.1";       // DEFINE YOUR OWN IAPs
    public const string IAP_ENERGY_PACK_2           = "yoursharingeconomyapp.rent.with.energy.2";       // DEFINE YOUR OWN IAPs
    public const string IAP_ENERGY_PACK_3           = "yoursharingeconomyapp.rent.with.energy.3";       // DEFINE YOUR OWN IAPs
    public const string IAP_ENERGY_PACK_4           = "yoursharingeconomyapp.rent.with.energy.4";       // DEFINE YOUR OWN IAPs
    public const string IAP_POST_OFFER_NO_WAIT      = "yoursharingeconomyapp.post.a.new.offer";         // DEFINE YOUR OWN IAPs
    public const string IAP_CREATE_NEW_REQUEST      = "yoursharingeconomyapp.create.a.new.request";     // DEFINE YOUR OWN IAPs

    // ----------------------------------------------
    // EVENTS
    // ----------------------------------------------
    public const string EVENT_IAP_CALL_PURCHASE_RENT_PROVIDER           = "EVENT_IAP_CALL_PURCHASE_RENT_PROVIDER";
    public const string EVENT_IAP_CALL_PURCHASE_POST_OFFER_NO_WAIT      = "EVENT_IAP_CALL_PURCHASE_POST_OFFER_NO_WAIT";
    public const string EVENT_IAP_CALL_PURCHASE_CREATE_NEW_REQUEST      = "EVENT_IAP_CALL_PURCHASE_CREATE_NEW_REQUEST";

    public const string EVENT_IAP_CONFIRMATION      = "EVENT_IAP_CONFIRMATION";

    public const string EVENT_IAP_CODE_CONFIRMATION = "EVENT_IAP_CODE_CONFIRMATION";

    // ----------------------------------------------
    // SINGLETON
    // ----------------------------------------------
    private static IAPController _instance;

    public static IAPController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<IAPController>();
            }
            return _instance;
        }
    }

    // ----------------------------------------------
    // MEMBERS
    // ----------------------------------------------
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    private string m_currentCodeTransaction = "";
    private string m_currentEventIAP = "";
    private string m_currentIdProduct = "";

    // ----------------------------------------------
    // GETTERS/SETTERS
    // ----------------------------------------------

    // ----------------------------------------------
    // CONSTRUCTOR
    // ----------------------------------------------	
    // -------------------------------------------
    /* 
	 * Initialitzation
	 */
    public void Init()
    {
        if (m_StoreController == null)
        {
            if (IsInitialized())
            {
                return;
            }

            BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);

#if ENABLED_FACEBOOK
            Debug.Log("Nothing to configure");
#else
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // ********************
            // DEFINE YOUR OWN IAPs
            // ********************
            builder.AddProduct(IAP_ENERGY_PACK_1, ProductType.Consumable);
            builder.AddProduct(IAP_ENERGY_PACK_2, ProductType.Consumable);
            builder.AddProduct(IAP_ENERGY_PACK_3, ProductType.Consumable);
            builder.AddProduct(IAP_ENERGY_PACK_4, ProductType.Consumable);
            builder.AddProduct(IAP_POST_OFFER_NO_WAIT, ProductType.Consumable);
            builder.AddProduct(IAP_CREATE_NEW_REQUEST, ProductType.Consumable);

            UnityPurchasing.Initialize(this, builder);
#endif
        }
    }

    // -------------------------------------------
    /* 
     * Destroy
     */
    public void Destroy()
    {
        BasicEventController.Instance.BasicEvent -= OnBasicEvent;
        DestroyObject(_instance.gameObject);
        _instance = null;
    }

    // -------------------------------------------
    /* 
	 * IsInitialized
	 */
    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    // -------------------------------------------
    /* 
	 * OnInitialized
	 */
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;

        // Purchasing has succeeded initializing. Collect our Purchasing references.
        if (ScreenController.Instance.DebugIAPs)
        {
            Debug.Log("OnInitialized: PASS+++++++++++++++++");
            Debug.Log("(m_StoreController!=null)[" + (m_StoreController != null) + "]");
            Debug.Log("(m_StoreExtensionProvider!=null)[" + (m_StoreExtensionProvider != null) + "]");
        }
    }

    // -------------------------------------------
    /* 
	 * OnInitializeFailed
	 */
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        if (ScreenController.Instance.DebugIAPs) Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    // -------------------------------------------
    /* 
	 * BuyProductID
	 */
    private void BuyProductID(string _productId)
    {
#if ENABLED_FACEBOOK
        FacebookController.Instance.PurchaseIAP(_productId);
#else
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(_productId);

            if (product != null && product.availableToPurchase)
            {
                if (ScreenController.Instance.DebugIAPs)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                }
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                if (ScreenController.Instance.DebugIAPs) Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            if (ScreenController.Instance.DebugIAPs) Debug.Log("BuyProductID FAIL. Not initialized.");
        }
#endif
    }

    // -------------------------------------------
    /* 
	 * RestorePurchases
	 */
    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            if (ScreenController.Instance.DebugIAPs) Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // We are not running on an Apple device. No work is necessary to restore purchases.
        if (ScreenController.Instance.DebugIAPs) Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
    }

    // -------------------------------------------
    /* 
	 * ProcessPurchase
	 */
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (ScreenController.Instance.DebugIAPs) Debug.Log(string.Format("ProcessPurchase: SUCCESS. Product: '{0}'", args.purchasedProduct.definition.id));
        BasicEventController.Instance.DispatchBasicEvent(EVENT_IAP_CONFIRMATION, true, args.purchasedProduct.definition.id);
        return PurchaseProcessingResult.Complete;
    }


    // -------------------------------------------
    /* 
	 * OnPurchaseFailed
	 */
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        if (ScreenController.Instance.DebugIAPs) Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        BasicEventController.Instance.DispatchBasicEvent(EVENT_IAP_CONFIRMATION, false, product.definition.id);
    }

    // -------------------------------------------
    /* 
	 * OnBasicEvent
	 */
    private void OnBasicEvent(string _nameEvent, params object[] _list)
    {
        if (_nameEvent == EVENT_IAP_CONFIRMATION)
        {
            bool success = (bool)_list[0];
            string iapID = (string)_list[1];
            if (ScreenController.Instance.DebugIAPs) Debug.Log("EVENT_IAP_CONFIRMATION::success[" + success + "]::iapID[" + iapID + "]");
            if ((iapID.IndexOf(IAP_ENERGY_PACK_1) !=-1) ||
                (iapID.IndexOf(IAP_ENERGY_PACK_2) != -1) ||
                (iapID.IndexOf(IAP_ENERGY_PACK_3) != -1) ||
                (iapID.IndexOf(IAP_ENERGY_PACK_4) != -1))
            {
                if (success)
                {
                    int rentValue = int.Parse(iapID.Substring(iapID.Length - 1, 1));
                    BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_IAP_CALL_PURCHASE_RENT_PROVIDER, success, rentValue, RJEncryptor.Encrypt(m_currentCodeTransaction, false));
                }
                else
                {
                    BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_IAP_CALL_PURCHASE_RENT_PROVIDER, success);
                }
            }
            else
            {
                if (iapID.IndexOf(IAP_POST_OFFER_NO_WAIT) != -1)
                {
                    BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_IAP_CALL_PURCHASE_POST_OFFER, success, RJEncryptor.Encrypt(m_currentCodeTransaction, false));
                }
                else
                {
                    if (iapID.IndexOf(IAP_CREATE_NEW_REQUEST) != -1)
                    {
                        BasicEventController.Instance.DispatchBasicEvent(UsersController.EVENT_USER_IAP_CALL_PURCHASE_NEW_REQUEST, success, RJEncryptor.Encrypt(m_currentCodeTransaction, false));
                    }
                    else
                    {
                        if (!success)
                        {
                            BasicEventController.Instance.DispatchBasicEvent(ScreenInformationView.EVENT_SCREENINFORMATION_FORCE_DESTRUCTION_POPUP);
                            string title = LanguageController.Instance.GetText("message.error");
                            string description = LanguageController.Instance.GetText("message.iap.failure.any.operation");
                            ScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, TypePreviousActionEnum.KEEP_CURRENT_SCREEN, title, description, null, "");
                        }
                    }
                }
            }
        }
        if (_nameEvent == EVENT_IAP_CALL_PURCHASE_RENT_PROVIDER)
        {
            m_currentEventIAP = EVENT_IAP_CALL_PURCHASE_RENT_PROVIDER;
            m_currentIdProduct = (string)_list[0];
            m_currentCodeTransaction = Utilities.RandomCodeGeneration(UsersController.Instance.CurrentUser.Id.ToString());
            CommController.Instance.IAPRegisterCode(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, RJEncryptor.Encrypt(m_currentCodeTransaction, false));            
        }
        if (_nameEvent == EVENT_IAP_CALL_PURCHASE_POST_OFFER_NO_WAIT)
        {
            m_currentEventIAP = EVENT_IAP_CALL_PURCHASE_POST_OFFER_NO_WAIT;
            m_currentCodeTransaction = Utilities.RandomCodeGeneration(UsersController.Instance.CurrentUser.Id.ToString());
            CommController.Instance.IAPRegisterCode(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, RJEncryptor.Encrypt(m_currentCodeTransaction,false));
        }
        if (_nameEvent == EVENT_IAP_CALL_PURCHASE_CREATE_NEW_REQUEST)
        {
            m_currentEventIAP = EVENT_IAP_CALL_PURCHASE_CREATE_NEW_REQUEST;
            m_currentCodeTransaction = Utilities.RandomCodeGeneration(UsersController.Instance.CurrentUser.Id.ToString());
            CommController.Instance.IAPRegisterCode(UsersController.Instance.CurrentUser.Id, UsersController.Instance.CurrentUser.Password, RJEncryptor.Encrypt(m_currentCodeTransaction, false));
        }
        if (_nameEvent == EVENT_IAP_CODE_CONFIRMATION)
        {
            if ((bool)_list[0])
            {
                if (RJEncryptor.Encrypt(m_currentCodeTransaction, false) == (string)_list[1])
                {
                    if (m_currentEventIAP == EVENT_IAP_CALL_PURCHASE_RENT_PROVIDER)
                    {
                        if (m_currentIdProduct.Length > 0)
                        {
                            BuyProductID(m_currentIdProduct);
                        }
                    }
                    if (m_currentEventIAP == EVENT_IAP_CALL_PURCHASE_POST_OFFER_NO_WAIT)
                    {
                        BuyProductID(IAP_POST_OFFER_NO_WAIT);
                    }
                    if (m_currentEventIAP == EVENT_IAP_CALL_PURCHASE_CREATE_NEW_REQUEST)
                    {
                        BuyProductID(IAP_CREATE_NEW_REQUEST);
                    }
                    m_currentEventIAP = "";
                    m_currentIdProduct = "";
                }
            }
        }
    }
}

