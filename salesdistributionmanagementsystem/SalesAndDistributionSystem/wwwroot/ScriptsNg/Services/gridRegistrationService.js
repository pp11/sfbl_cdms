ngApp.service("gridregistrationservice", function ($http) {
    //Function to call return grid registration
    this.GridRegistration= function(reportHeader) {
        return  data = {
            // EXPORT : START
            enableFiltering: true,
            enableSorting: true,
            enableGridMenu: true,
            enableSelectAll: true,
            enableColumnMenus: true,
            enableAutoFitColumns: false,
            paginationPageSizes:[10,25, 50, 75],
            paginationPageSize: 10,
            exporterCsvFilename: 'myFile.csv',
            exporterPdfDefaultStyle: { fontSize: 7 },
            exporterPdfTableStyle: { margin: [05, 05, 05 , 05] },
            exporterPdfTableHeaderStyle: { fontSize: 8, bold: true, italics: false, color: 'black' },
            exporterPdfHeader: { text: reportHeader, style: 'headerStyle', fontSize: 14, bold: true },
            exporterPdfFooter: function (currentPage, pageCount) {
                return { text:'Powered By: Square Informatix Ltd.     '+ currentPage.toString() + ' of ' + pageCount.toString(), style: 'footerStyle' };
            },
            exporterPdfCustomFormatter: function (docDefinition) {
                docDefinition.styles.headerStyle = { margin: [10, 10, 10, 10], alignment: 'center' };
                docDefinition.styles.footerStyle = { fontSize: 8, bold: false, alignment: 'center' };
                return docDefinition;
            },
            exporterPdfOrientation: 'landscape',
            exporterPdfPageSize: 'LETTER',
            exporterPdfMaxGridWidth: 500,
            exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
            exporterExcelFilename: 'myFile.xlsx',
            exporterExcelSheetName: 'Sheet1',
            // EXPORT : END
            enableFiltering: true,
            showGridFooter: true,
            // Default: End
           
        }

    }
}); 