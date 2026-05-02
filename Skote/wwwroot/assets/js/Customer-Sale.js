(function () {
    "use strict";

    const prescriptionSelect = document.getElementById("prescription-select");
    const prescriptionBox = document.getElementById("prescription-box");

    const paymentBody = document.getElementById("payments-body");
    const paymentTemplate = document.getElementById("payment-line-template");
    const btnAddPayment = document.getElementById("btn-add-payment");
    const saleTotalInput = document.getElementById("sale-total");
    const saleRemainingInput = document.getElementById("sale-remaining");
    const form = document.getElementById("customer-sale-form");

    function wirePriceAutoFill() {
        document.querySelectorAll(".optical-item-select").forEach(select => {
            select.addEventListener("change", function () {
                const selected = select.options[select.selectedIndex];
                const targetId = select.dataset.priceTarget;
                const target = document.getElementById(targetId);

                if (target && selected) {
                    const price = Number(selected.getAttribute("data-price") || 0);
                    if (!Number(target.value || 0)) {
                        target.value = price.toFixed(2);
                    }
                }

                calculateSaleTotal();
            });
        });

        document.querySelectorAll(".line-qty, .line-price").forEach(input => {
            input.addEventListener("input", calculateSaleTotal);
        });
    }

    async function loadPrescription() {
        const id = prescriptionSelect?.value;
        if (!id) {
            prescriptionBox.innerHTML = '<div class="text-muted">اختر الروشتة لعرض التفاصيل</div>';
            return;
        }

        //const response = await fetch(`/CustomerSales/GetPrescription?id=${encodeURIComponent(id)}`);
        //const result = await response.json();

        if (!result.ok) {
            prescriptionBox.innerHTML = '<div class="text-danger">تعذر تحميل بيانات الروشتة</div>';
            return;
        }

        const p = result.data;

        prescriptionBox.innerHTML = `
            <div class="row g-2">
                <div class="col-12"><strong>Date:</strong> ${p.date || "-"}</div>
                <div class="col-12"><strong>Doctor:</strong> ${p.doctor || "-"}</div>
                <div class="col-md-6"><strong>Right:</strong> Sph ${p.rightSph ?? "-"} | Cyl ${p.rightCyl ?? "-"} | Axis ${p.rightAxis ?? "-"}</div>
                <div class="col-md-6"><strong>Left:</strong> Sph ${p.leftSph ?? "-"} | Cyl ${p.leftCyl ?? "-"} | Axis ${p.leftAxis ?? "-"}</div>
                <div class="col-md-4"><strong>Add:</strong> ${p.addValue ?? "-"}</div>
                <div class="col-md-4"><strong>IPD:</strong> ${p.ipd ?? "-"}</div>
                <div class="col-md-4"><strong>S Height:</strong> ${p.sHeight ?? "-"}</div>
                <div class="col-12"><strong>Remarks:</strong> ${p.remarks || "-"}</div>
            </div>
        `;
    }

    function calculateSaleTotal() {
        let total = 0;

        total += getLineTotal("FrameItemId", "FrameQty", "FrameUnitPrice");
        total += getLineTotal("LensItemId", "LensQty", "LensUnitPrice");
        total += getLineTotal("ContactLensItemId", "ContactLensQty", "ContactLensUnitPrice");
        total += getLineTotal("AccessoryItemId", "AccessoryQty", "AccessoryUnitPrice");

        saleTotalInput.value = total.toFixed(2);
        calculatePayments();
    }

    function getLineTotal(itemIdField, qtyField, priceField) {
        const itemId = document.getElementById(itemIdField)?.value;
        const qty = Number(document.getElementById(qtyField)?.value || 0);
        const price = Number(document.getElementById(priceField)?.value || 0);

        if (!itemId || qty <= 0 || price <= 0) return 0;
        return qty * price;
    }

    function bindPaymentRow(row) {
        row.querySelector(".payment-amount").addEventListener("input", calculatePayments);
        row.querySelector(".btn-remove-payment").addEventListener("click", () => {
            row.remove();
            calculatePayments();
        });
    }

    function addPaymentRow() {
        const row = paymentTemplate.cloneNode(true);
        row.removeAttribute("id");
        row.classList.remove("d-none");

        bindPaymentRow(row);
        paymentBody.appendChild(row);
    }

    function calculatePayments() {
        const rows = paymentBody.querySelectorAll("tr");
        let totalPaid = 0;

        rows.forEach(row => {
            totalPaid += Number(row.querySelector(".payment-amount").value || 0);
        });

        const total = Number(saleTotalInput.value || 0);
        const remaining = total - totalPaid;

        saleRemainingInput.value = remaining.toFixed(2);

        if (Math.round(remaining * 100) !== 0) {
            saleRemainingInput.classList.add("text-danger");
            saleRemainingInput.classList.remove("text-success");
        } else {
            saleRemainingInput.classList.remove("text-danger");
            saleRemainingInput.classList.add("text-success");
        }
    }

    function rebuildPaymentIndexesBeforeSubmit() {
        const rows = paymentBody.querySelectorAll("tr");

        rows.forEach((row, index) => {
            const method = row.querySelector(".payment-method");
            const amount = row.querySelector(".payment-amount");

            method.name = `Payments[${index}].PaymentMethodId`;
            amount.name = `Payments[${index}].Amount`;
        });
    }

    if (prescriptionSelect) {
        prescriptionSelect.addEventListener("change", loadPrescription);
        if (prescriptionSelect.value) {
            loadPrescription();
        }
    }

    if (btnAddPayment) {
        btnAddPayment.addEventListener("click", addPaymentRow);
    }

    document.querySelectorAll("#payments-body tr").forEach(bindPaymentRow);

    wirePriceAutoFill();
    calculateSaleTotal();

    form.addEventListener("submit", function (e) {
        calculateSaleTotal();
        rebuildPaymentIndexesBeforeSubmit();

        const remaining = Number(saleRemainingInput.value || 0);
        if (Math.round(remaining * 100) !== 0) {
            e.preventDefault();
            alert("مجموع طرق الدفع يجب أن يساوي إجمالي البيع");
        }
    });
})();