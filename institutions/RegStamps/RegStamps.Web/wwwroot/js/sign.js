//function signText(text) {
//    if (window.event) {
//        window.event.cancelBubble = true;
//	}
//    return sign(text);
//}


//function sign(src) {
//    if (window.crypto && window.crypto.signText) {
//        return sign_NS(src);
//	}
//	if (isIE()) {
//		return sign_IE(src);
//	}
	
//	alert("Браузърът Ви не поддържа подписване! Моля, използвайте Mozilla Firefox версия 28 или по-висока или Internet Explorer!");
//	return "";
//}

//function sign_NS(src) {
//    result = crypto.signText(src, "ask");
//        if(result == 'error:userCancel' || result =='error:internalError' || result == 'error:noMatchingCert'){
//            alert("Грешка при подписването! Ако използвате електронен подпис на Информационно обслужване, моля вижте инструкцията им за настройка на Mozilla Firefox, стр. 5, т.2" + result);
//        return '';
//        }
//		return result;
//}

//// CAPICOM constants
//var CAPICOM_STORE_OPEN_READ_ONLY = 0;
//var CAPICOM_CURRENT_USER_STORE = 2;
//var CAPICOM_CERTIFICATE_FIND_SHA1_HASH = 0;
//var CAPICOM_CERTIFICATE_FIND_EXTENDED_PROPERTY = 6;
//var CAPICOM_CERTIFICATE_FIND_TIME_VALID = 9;
//var CAPICOM_CERTIFICATE_FIND_KEY_USAGE = 12;
//var CAPICOM_DIGITAL_SIGNATURE_KEY_USAGE = 0x00000080;
//var CAPICOM_AUTHENTICATED_ATTRIBUTE_SIGNING_TIME = 0;
//var CAPICOM_INFO_SUBJECT_SIMPLE_NAME = 0;
//var CAPICOM_ENCODE_BASE64 = 0;
//var CAPICOM_E_CANCELLED = -2138568446;
//var CERT_KEY_SPEC_PROP_ID = 6;

//function isIE() {
//	if ("ActiveXObject" in window){
//		return true;
//	}
//	return false;
//}

//function findCertificateByHash() {
//    try {
//        // instantiate the CAPICOM objects
//        var store = new ActiveXObject("CAPICOM.Store");
//        // open the current users personal certificate store
//        store.Open(CAPICOM_CURRENT_USER_STORE, "My", CAPICOM_STORE_OPEN_READ_ONLY);

//		var certificates = MyStore.Certificates.Select(); 
					
//        var signer = new ActiveXObject("CAPICOM.Signer");
//        signer.Certificate = certificates.Item(1);
//        return signer;
//    } catch (e) {
//        if (e.number != CAPICOM_E_CANCELLED) {
//            return new ActiveXObject("CAPICOM.Signer");
//        }
//    }
//}

//function sign_IE(src) {
//    try {
//        // instantiate the CAPICOM objects
//        var signedData = new ActiveXObject("CAPICOM.SignedData");
//        var timeAttribute = new ActiveXObject("CAPICOM.Attribute");

//        // Set the data that we want to sign
//        signedData.Content = src;
//        var signer = findCertificateByHash();

//        // Set the time in which we are applying the signature
//        var today = new Date();
//        timeAttribute.Name = CAPICOM_AUTHENTICATED_ATTRIBUTE_SIGNING_TIME;
//        timeAttribute.Value = today.getVarDate();
//        signer.AuthenticatedAttributes.Add(timeAttribute);

//        // Do the Sign operation
//        var signed = signedData.Sign(signer, true, CAPICOM_ENCODE_BASE64);
//		// Important: IE uses UTF-16LE to encode the signed data
//        return signed;
//    } catch (e) {
//        if (e.number != CAPICOM_E_CANCELLED) {
//            return new ActiveXObject("CAPICOM.Signer");
//        }
//    }
//    return "";
//}

function signWithNeispuo(stampId) {
    $.ajax({
        type: "POST",
        url: 'http://127.0.0.1:5339/api/certificate/sign',
        data: JSON.stringify({ "xml": "<xml><contents><stamp>" + stampId + "</stamp></contents></xml>" }),
        //"eik": "123456789"
        contentType: 'application/json; charset=utf-8',
        dataType: "text",
        success: successFunc,
        error: errorFunc
    });

    function successFunc(data) {
        // successFuncShow(data, field);
        // console.log(JSON.parse(data).contents)

        const xml = JSON.parse(data).contents;
        const parser = new DOMParser();
        const doc = parser.parseFromString(xml, "application/xml");
        // console.log(doc.getElementsByTagName("X509Certificate")[0].textContent)
        $("#signdata").val(doc.getElementsByTagName("X509Certificate")[0].textContent);
        if ($("#signdata").val() != "") {
            // SUCCESS SEND ELSign Data to server
            //$("#btn-modal-confirm").removeAttr("disabled");
            // $("#sign-status").show();
            $('.alert-success-fade-el').fadeIn(1000);
            $('.alert-danger-fade-el').fadeOut(1000);

            $('#send-request-form-id').submit();
        }
        else {
            $('.alert-success-fade-el').fadeOut(1000);
            $('.alert-danger-fade-el').text('Грешка при подписване').fadeIn(1000);
        }
    }

    function errorFunc() {
        $('.alert-danger-fade-el').text('Моля уверете се че са включени електронният подпис и локален сървър за достъп до електронни сертификати!').fadeIn(1000);
    }
}

function signLogin() {
    $.ajax({
        type: "POST",
        url: 'http://127.0.0.1:5339/api/certificate/sign',
        data: JSON.stringify({ "xml": "<xml><contents><stamp>123456</stamp></contents></xml>" }),
        //"eik": "123456789"
        contentType: 'application/json; charset=utf-8',
        dataType: "text",
        success: successFunc,
        error: errorFunc
    });

    function successFunc(data) {
        // successFuncShow(data, field);
        // console.log(JSON.parse(data).contents)

        const xml = JSON.parse(data).contents;
        const parser = new DOMParser();
        const doc = parser.parseFromString(xml, "application/xml");
        //console.log(doc.getElementsByTagName("X509Certificate")[0].textContent)
        $("#signature").val(doc.getElementsByTagName("X509Certificate")[0].textContent);
        if ($("#signature").val() != "") {
            // SUCCESS SEND ELSign Data to server
            //$("#btn-modal-confirm").removeAttr("disabled");
            // $("#sign-status").show();
            //$('.alert-success-fade-el').fadeIn(1000);
            //$('.alert-danger-fade-el').fadeOut(1000);

            $('#login-form-id').submit();
        }
        else {
            //$('.alert-success-fade-el').fadeOut(1000);
            //$('.alert-danger-fade-el').text('Грешка при подписване').fadeIn(1000);
        }
    }

    function errorFunc() {
        /*$('.alert-danger-fade-el').text('Моля уверете се че са включени електронният подпис и локален сървър за достъп до електронни сертификати!').fadeIn(1000);*/
    }
}