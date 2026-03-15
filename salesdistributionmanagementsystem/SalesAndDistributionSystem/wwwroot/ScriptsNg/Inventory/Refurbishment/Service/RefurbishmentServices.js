ngApp.service("refurbishmentServices", function($http) {
    this.getDistList = function () {
        return $http({
            method: 'GET',
            url: '../Refurbishment/GetDistList'
        })
    }
    this.getDistListWhileEdit = function (param) {
        return $http({
            method: 'GET',
            url: '../Refurbishment/GetDistListWhileEdit',
            params: param
        })
    }
    this.getProductsByChallan = function (param) {
        return $http({
            method: 'GET',
            url: '../Refurbishment/GetProductsByChallan',
            params: param
        })
    }
    this.addOrUpdate = function (model) {
        return $http({
            method: 'POST',
            url: "../Refurbishment/AddOrUpdate",
            dataType: 'application/json; charset=utf-8',
            data: JSON.stringify(model)
        });
    }
    this.approval = function (model) {
        return $http({
            method: 'POST',
            url: "../Refurbishment/Approval",
            dataType: 'application/json; charset=utf-8',
            data: JSON.stringify(model)
        });
    }
    this.getMstList = function (param) {
        return $http({
            method: 'GET',
            url: '../Refurbishment/GetMstList',
            params: param
        })
    }
    this.loadDtlByMstId = function (param) {
        return $http({
            method: 'GET',
            url: '../Refurbishment/LoadDtlByMstId',
            params: param
        })
    }

    this.loadDtlByMstId = function (param) {
        return $http({
            method: 'GET',
            url: '../Refurbishment/LoadDtlByMstId',
            params: param
        })
    }

    this.getManualDistList = function (param) {
        return $http({
            method: 'GET',
            url: '../Refurbishment/GetManualDistList',
            params: param
        })
    }
    this.getManualProductList = function (param) {
        return $http({
            method: 'GET',
            url: '../Refurbishment/GetManualProductList',
            params: param
        })
    }

    this.getClaims = function (param) {
        return $http({
            method: 'GET',
            url: '../Refurbishment/GetClaims',
            params: param
        })
    }
    this.getProductsByClaimNo = function (param) {
        return $http({
            method: 'GET',
            url: '../Refurbishment/GetProductsByClaimNo',
            params: param
        })
    }
    this.addOrUpdateFinalization = function (model) {
        return $http({
            method: 'POST',
            url: "../Refurbishment/AddOrUpdateFinalization",
            dataType: 'application/json; charset=utf-8',
            data: JSON.stringify(model)
        });
    }
    this.getFinalizationMstList = function (param) {
        return $http({
            method: 'GET',
            url: '../Refurbishment/GetFinalizationMstList',
            params: param
        })
    }
    this.getClaimsWhileEdit = function (param) {
        return $http({
            method: 'GET',
            url: '../Refurbishment/GetClaimsWhileEdit',
            params: param
        })
    }
    this.loadFinalizationDtlByMstId = function (param) {
        return $http({
            method: 'GET',
            url: '../Refurbishment/loadFinalizationDtlByMstId',
            params: param
        })
    }
    this.finalizationApproval = function (model) {
        return $http({
            method: 'POST',
            url: "../Refurbishment/FinalizationApproval",
            dataType: 'application/json; charset=utf-8',
            data: JSON.stringify(model)
        });
    }
    this.getManualProductsWithStock = function (param) {
        return $http({
            method: 'GET',
            url: '../Refurbishment/GetManualProductsWithStock',
            params: param
        })
    }
})