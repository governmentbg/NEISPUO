/*
    Превод на български език на част от текстовете около Telerik report viewer-а.

    Указания от Telerik за превеждане на текстовете на report viewer-а:
    а) Ако се използва Angular компонентата.
    https://docs.telerik.com/reporting/angular-report-viewer-localization
    б) Ако се използва jQuery вариантът. Тук се виждат също ключовете и текстовете по подразбиране.
    https://docs.telerik.com/reporting/html5-report-viewer-localization
*/

export class StringResources {
    static bulgarian = {
        // Warning and error string resources
        // Missing or invalid parameter value. Please input valid data for all parameters.
        missingOrInvalidParameter:
            'Стойността на параметъра липсва или е невалидна. Моля въведете всички параметри правилно.',
        // Please input a valid value.
        invalidParameter: 'Моля, въведете валидна стойност.',
        // Please input a valid date.
        invalidDateTimeValue: 'Моля, въведете валидна дата.',
        // Parameter value cannot be empty.
        parameterIsEmpty: 'Параметърът не може да остане празен.',
        // Cannot validate parameter of type {type}.
        cannotValidateType: 'Не се поддържа валидиране на параметри от тип {type}.',
        // Loading...
        loadingFormats: 'Зареждане...',
        // Loading report...
        loadingReport: 'Данните се зареждат...',
        // Preparing document to download. Please wait...
        preparingDownload: 'Подготовка за изтегляне. Моля, изчакайте...',
        // Preparing document to print. Please wait...
        preparingPrint: 'Подготовка за печат. Моля, изчакайте...',
        // Error loading the report viewer's templates. (templateUrl = '{0}').
        errorLoadingTemplates: 'Грешка при зареждане на шаблона за report viewer-а. (templateUrl = "{0}").',
        // Cannot access the Reporting REST service. (serviceUrl = '{0}'). Make sure the service address is correct and enable CORS if needed. (https://enable-cors.org)
        errorServiceUrl:
            'Няма достъп до REST услугата за справки. (serviceUrl = "{0}"). Проверете адреса и настройте CORS ако е нужно. (https://enable-cors.org)',
        // {0} pages loaded so far...
        loadingReportPagesInProgress: 'Заредени {0} страници...',
        // Done. Total {0} pages loaded.
        loadedReportPagesComplete: 'Готово. Заредени общо {0} страници.',
        // No page to display.
        noPageToDisplay: 'Няма страница за показване.',
        // Click 'Refresh' to restore client session.
        clientExpired: 'Натиснете "Презареждане", за да възобновите сесията.',
        // Report processing was canceled.
        renderingCanceled: 'Вие отменихте генерирането на бланката.',
        /*
        controllerNotInitialized: 'Controller is not initialized.',
        noReportInstance: 'No report instance.',
        missingTemplate: '!obsolete resource!',
        noReport: 'No report.',
        noReportDocument: 'No report document.',
        errorDeletingReportInstance: `Error deleting report instance: '{0}'.`,
        errorRegisteringViewer: 'Error registering the viewer with the service.',
        noServiceClient: 'No serviceClient has been specified for this controller.',
        errorRegisteringClientInstance: 'Error registering client instance.',
        errorCreatingReportInstance: "Error creating report instance (Report = '{0}').",
        errorCreatingReportDocument: "Error creating report document (Report = '{0}'; Format = '{1}').",
        unableToGetReportParameters: 'Unable to get report parameters.',
        errorObtainingAuthenticationToken: 'Error obtaining authentication token.',
        promisesChainStopError: 'Error shown. Throwing promises chain stop error.',
        */

        // Viewer template string resources
        // clear selection
        parameterEditorSelectNone: 'размаркирай всичко',
        // select all
        parameterEditorSelectAll: 'маркирай всичко',
        // Preview
        parametersAreaPreviewButton: 'Преглед',

        // Navigate Backward
        menuNavigateBackwardText: 'Назад',
        // Navigate Backward
        menuNavigateBackwardTitle: 'Назад',
        // Navigate Forward
        menuNavigateForwardText: 'Напред',
        // Navigate Forward
        menuNavigateForwardTitle: 'Напред',
        // Stop Rendering
        menuStopRenderingText: 'Спри генерирането',
        // Stop Rendering
        menuStopRenderingTitle: 'Спри генерирането',
        // Refresh
        menuRefreshText: 'Презареждане',
        // Refresh
        menuRefreshTitle: 'Презареждане',
        // First Page
        menuFirstPageText: 'Първа страница',
        // First Page
        menuFirstPageTitle: 'Първа страница',
        // Last Page
        menuLastPageText: 'Последна страница',
        // Last Page
        menuLastPageTitle: 'Последна страница',
        // Previous Page
        menuPreviousPageTitle: 'Предишна страница',
        // Next Page
        menuNextPageTitle: 'Следваща страница',
        // Page Number Selector
        menuPageNumberTitle: 'Отиди на страница...',
        // Toggle Document Map
        menuDocumentMapTitle: 'Карта на документа (вкл/изкл.)',
        // Toggle Parameters Area
        menuParametersAreaTitle: 'Параметри (вкл/изкл.)',
        // Zoom In
        menuZoomInTitle: 'Увеличаване',
        // Zoom Out
        menuZoomOutTitle: 'Намаляване',
        // Toggle FullPage/PageWidth
        menuPageStateTitle: 'Цяла страница / по ширина / 1:1',
        // Print...
        menuPrintText: 'Печат...',
        // Print
        menuPrintTitle: 'Печат',
        // Toggle Continuous Scrolling
        menuContinuousScrollText: 'Постоянно скролиране (вкл/изкл.)',
        // Toggle Continuous Scrolling
        menuContinuousScrollTitle: 'Постоянно скролиране (вкл/изкл.)',
        // Send an email
        menuSendMailText: 'Изпращане на e-mail',
        // Send an email
        menuSendMailTitle: 'Изпращане на e-mail',
        // Export
        menuExportText: 'Запис като...',
        // Export
        menuExportTitle: 'Запис като...',
        // Toggle Print Preview
        menuPrintPreviewText: 'Печатен вид (вкл/изкл.)',
        // Toggle Print Preview
        menuPrintPreviewTitle: 'Печатен вид (вкл/изкл.)',
        // Search
        menuSearchText: 'Търсене',
        // Toggle Search
        menuSearchTitle: 'Търсене (вкл/изкл.)',
        // Toggle Side Menu
        menuSideMenuTitle: 'Странѝчно меню (вкл/изкл.)',

        // Send Email dialog resources
        // From:
        sendEmailFromLabel: 'От:',
        // To:
        sendEmailToLabel: 'До:',
        // CC:
        sendEmailCCLabel: 'Копие до:',
        // Subject:
        sendEmailSubjectLabel: 'Тема:',
        // Format:
        sendEmailFormatLabel: 'Формат:',
        // Send
        sendEmailSendLabel: 'Изпращане',
        // Cancel
        sendEmailCancelLabel: 'Отказ',
        // Send Email
        sendEmailDialogTitle: 'Изпращане на e-mail',
        // Email field is required
        sendEmailValidationEmailRequired: 'Моля, въведете e-mail адрес',
        // Email format is not valid
        sendEmailValidationEmailFormat: 'Невалиден e-mail адрес',
        // The field accepts a single email address only
        sendEmailValidationSingleEmail: 'Допуска се само един e-mail адрес',
        // Format field is required
        sendEmailValidationFormatRequired: 'Моля, изберете формат',
        // Error sending report document (Report = "{0}").
        errorSendingDocument: 'Грешка при изпращане (Report = "{0}").',

        // Search dialog resources
        // Search in report contents
        searchDialogTitle: 'Търсене в бланката',
        // searching...
        searchDialogSearchInProgress: 'търсене...',
        // No results
        searchDialogNoResultsLabel: 'Търсенето не намери нищо',
        // Result {0} of {1}
        searchDialogResultsFormatLabel: 'Резултат {0} от {1}',
        // Stop Search
        searchDialogStopTitle: 'Спри търсенето',
        // Navigate Up
        searchDialogNavigateUpTitle: 'Предишен резултат',
        // Navigate Down
        searchDialogNavigateDownTitle: 'Следващ резултат',
        // Match Case
        searchDialogMatchCaseTitle: 'Точно съвпадение на малки и главни букви',
        // Match Whole Word
        searchDialogMatchWholeWordTitle: 'Само цели думи',
        // Use Regex
        searchDialogUseRegexTitle: 'Регулярен израз',
        // Find
        searchDialogCaptionText: 'Търси следния текст',
        // page
        searchDialogPageText: 'страница'

        // Accessibility string resources
        /*
        ariaLabelPageNumberSelector: 'Page number selector. Showing page {0} of {1}.',
        ariaLabelPageNumberEditor: 'Page number editor',
        ariaLabelExpandable: 'Expandable',
        ariaLabelSelected: 'Selected',
        ariaLabelParameter: 'parameter',
        ariaLabelErrorMessage: 'Error message',
        ariaLabelParameterInfo: 'Contains {0} options',
        ariaLabelMultiSelect: 'Multiselect',
        ariaLabelMultiValue: 'Multivalue',
        ariaLabelSingleValue: 'Single value',
        ariaLabelParameterDateTime: 'DateTime',
        ariaLabelParameterString: 'String',
        ariaLabelParameterNumerical: 'Numerical',
        ariaLabelParameterBoolean: 'Boolean',
        ariaLabelParametersAreaPreviewButton: 'Preview the report',
        ariaLabelMainMenu: 'Main menu',
        ariaLabelCompactMenu: 'Compact menu',
        ariaLabelSideMenu: 'Side menu',
        ariaLabelDocumentMap: 'Document map area',
        ariaLabelDocumentMapSplitter: 'Document map area splitbar.',
        ariaLabelParametersAreaSplitter: 'Parameters area splitbar.',
        ariaLabelPagesArea: 'Report contents area',
        ariaLabelSearchDialogArea: 'Search area',
        ariaLabelSendEmailDialogArea: 'Send email area',
        ariaLabelSearchDialogStop: 'Stop search',
        ariaLabelSearchDialogOptions: 'Search options',
        ariaLabelSearchDialogNavigateUp: 'Navigate up',
        ariaLabelSearchDialogNavigateDown: 'Navigate down',
        ariaLabelSearchDialogMatchCase: 'Match case',
        ariaLabelSearchDialogMatchWholeWord: 'Match whole word',
        ariaLabelSearchDialogUseRegex: 'Use regex',
        ariaLabelMenuNavigateBackward: 'Navigate backward',
        ariaLabelMenuNavigateForward: 'Navigate forward',
        ariaLabelMenuStopRendering: 'Stop rendering',
        ariaLabelMenuRefresh: 'Refresh',
        ariaLabelMenuFirstPage: 'First page',
        ariaLabelMenuLastPage: 'Last page',
        ariaLabelMenuPreviousPage: 'Previous page',
        ariaLabelMenuNextPage: 'Next page',
        ariaLabelMenuPageNumber: 'Page number selector',
        ariaLabelMenuDocumentMap: 'Toggle document map',
        ariaLabelMenuParametersArea: 'Toggle parameters area',
        ariaLabelMenuZoomIn: 'Zoom in',
        ariaLabelMenuZoomOut: 'Zoom out',
        ariaLabelMenuPageState: 'Toggle FullPage/PageWidth',
        ariaLabelMenuPrint: 'Print',
        ariaLabelMenuContinuousScroll: 'Continuous scrolling',
        ariaLabelMenuSendMail: 'Send an email',
        ariaLabelMenuExport: 'Export',
        ariaLabelMenuPrintPreview: 'Toggle print preview',
        ariaLabelMenuSearch: 'Search in report contents',
        ariaLabelMenuSideMenu: 'Toggle side menu',
        ariaLabelSendEmailFrom: 'From email address',
        ariaLabelSendEmailTo: 'Recipient email address',
        ariaLabelSendEmailCC: 'Carbon Copy email address',
        ariaLabelSendEmailSubject: 'Email subject:',
        ariaLabelSendEmailFormat: 'Report format:',
        ariaLabelSendEmailSend: 'Send email',
        ariaLabelSendEmailCancel: 'Cancel sending email',
        */
    };
}
