(function () {
    "use strict";
    const btnSavePos = document.getElementById("btn-save-pos");
    const btnReprintLast = document.getElementById("btn-reprint-last");
    let lastPrintUrl = null;
    let isSaving = false;
    const body = document.getElementById("pos-lines-body");
    const template = document.getElementById("pos-line-template");
    const searchInput = document.getElementById("item-search");
    const btnAddLine = document.getElementById("btn-add-line");
    const btnClearLines = document.getElementById("btn-clear-lines");
    const form = document.getElementById("pos-form");
    const warehouseSelect = document.getElementById("WarehouseId");
    const paymentsBody = document.getElementById("pos-payments-body");
    const paymentTemplate = document.getElementById("payment-line-template");
    const btnAddPayment = document.getElementById("btn-add-payment");
    const totalPaidInput = document.getElementById("total-paid");
    const remainingPaidInput = document.getElementById("remaining-paid");
    const subTotalInput = document.getElementById("SubTotal");
    const discountInput = document.getElementById("DiscountAmount");
    const taxInput = document.getElementById("TaxAmount");
    const netInput = document.getElementById("NetAmount");
    function bindPaymentEvents(row) {
        row.querySelector(".payment-amount").addEventListener("input", calculatePaymentTotals);
        row.querySelector(".payment-method").addEventListener("change", calculatePaymentTotals);

        row.querySelector(".btn-remove-payment").addEventListener("click", () => {
            row.remove();
            calculatePaymentTotals();
        });
    }

    function addPaymentRow() {
        const row = paymentTemplate.cloneNode(true);
        row.removeAttribute("id");
        row.classList.remove("d-none");
        bindPaymentEvents(row);
        paymentsBody.appendChild(row);
        calculatePaymentTotals();
    }

    function calculatePaymentTotals() {
        const rows = paymentsBody.querySelectorAll("tr");
        let totalPaid = 0;

        rows.forEach(row => {
            totalPaid += Number(row.querySelector(".payment-amount").value || 0);
        });

        const net = Number(netInput.value || 0);
        const remaining = net - totalPaid;

        totalPaidInput.value = totalPaid.toFixed(2);
        remainingPaidInput.value = remaining.toFixed(2);

        if (Math.round(remaining * 100) !== 0) {
            remainingPaidInput.classList.add("text-danger");
            remainingPaidInput.classList.remove("text-success");
        } else {
            remainingPaidInput.classList.remove("text-danger");
            remainingPaidInput.classList.add("text-success");
        }
    }
    function resetPosForm(nextInvoiceNumber) {
        body.innerHTML = "";
        paymentsBody.innerHTML = "";
        addPaymentRow();

        calculateTotals();

        document.getElementById("InvoiceNumber").value = nextInvoiceNumber || "";
        document.getElementById("CustomerName").value = "عميل نقدي";
        document.getElementById("Notes").value = "";

        searchInput.value = "";
        searchInput.focus();
    }
    function buildHiddenLines() {
        document.querySelectorAll(".dynamic-pos-hidden").forEach(x => x.remove());

        const rows = body.querySelectorAll("tr");
        rows.forEach((row, index) => {
            appendHidden(`Lines[${index}].ItemId`, row.querySelector(".item-id").value);
            appendHidden(`Lines[${index}].ItemCode`, row.querySelector(".item-code").value);
            appendHidden(`Lines[${index}].ItemNameAr`, row.querySelector(".item-name").value);
            appendHidden(`Lines[${index}].Quantity`, row.querySelector(".qty").value);
            appendHidden(`Lines[${index}].UnitPrice`, row.querySelector(".price").value);
            appendHidden(`Lines[${index}].DiscountAmount`, row.querySelector(".discount").value);
            appendHidden(`Lines[${index}].TaxAmount`, row.querySelector(".tax").value);
            appendHidden(`Lines[${index}].LineTotal`, row.querySelector(".line-total").value);
        });

        const paymentRows = paymentsBody.querySelectorAll("tr");
        paymentRows.forEach((row, index) => {
            appendHidden(`Payments[${index}].PaymentMethodId`, row.querySelector(".payment-method").value);
            appendHidden(`Payments[${index}].Amount`, row.querySelector(".payment-amount").value);
        });
    }

    async function savePosAjax() {
        if (isSaving) return;

        const rows = body.querySelectorAll("tr");

        if (!rows.length) {
            alert("أضف صنفًا واحدًا على الأقل");
            return;
        }

        const invalidRow = Array.from(rows).find(row => row.classList.contains("table-danger"));
        if (invalidRow) {
            alert("يوجد صنف كميته أكبر من الرصيد المتاح");
            return;
        }
        const remaining = Number(remainingPaidInput.value || 0);
        if (Math.round(remaining * 100) !== 0) {
            alert("مجموع طرق الدفع يجب أن يساوي صافي الفاتورة");
            return;
        }
        isSaving = true;
        btnSavePos.disabled = true;
        btnSavePos.textContent = "جاري الحفظ...";

        try {
            buildHiddenLines();
            calculateTotals();

            const formData = new FormData(form);
          
            const response = await fetch("/Pos/SaveAjax", {
                method: "POST",
                body: formData
            });

            const result = await response.json();

            if (!result.ok) {
                alert(result.message || "فشل حفظ الفاتورة");
                return;
            }

            lastPrintUrl = result.printUrl;

            if (result.printUrl) {
                window.open(result.printUrl, "_blank");
            }

            resetPosForm(result.nextInvoiceNumber);
            alert(result.message || "تم الحفظ بنجاح");
        } catch (error) {
            console.error(error);
            alert("حدث خطأ أثناء حفظ الفاتورة");
        } finally {
            isSaving = false;
            btnSavePos.disabled = false;
            btnSavePos.textContent = "حفظ الفاتورة";
        }
    }
    function addLine(item) {
        // لو الصنف موجود بالفعل، نزود الكمية بدل تكرار سطر جديد
        if (item && mergeIfExists(item)) {
            return;
        }

        const row = template.cloneNode(true);
        row.removeAttribute("id");
        row.classList.remove("d-none");

        const itemId = row.querySelector(".item-id");
        const itemCode = row.querySelector(".item-code");
        const itemName = row.querySelector(".item-name");
        const availableQty = row.querySelector(".available-qty");
        const qty = row.querySelector(".qty");
        const price = row.querySelector(".price");
        const discount = row.querySelector(".discount");
        const tax = row.querySelector(".tax");

        if (item) {
            itemId.value = item.id || "";
            itemCode.value = item.code || "";
            itemName.value = item.name || "";
            availableQty.value = Number(item.availableQty || 0).toFixed(2);
            price.value = Number(item.salePrice || 0).toFixed(2);

            if (item.isTaxable && Number(item.taxRate || 0) > 0) {
                const initialTax = ((Number(qty.value) * Number(price.value)) * Number(item.taxRate)) / 100;
                tax.value = initialTax.toFixed(2);
            }
        }

        bindLineEvents(row);
        calculateLine(row);
        body.appendChild(row);

        row.querySelector(".qty").focus();
        row.querySelector(".qty").select();
    }

    function mergeIfExists(item) {
        const rows = body.querySelectorAll("tr");
        for (const row of rows) {
            const currentId = row.querySelector(".item-id")?.value;
            if (currentId && currentId === item.id) {
                const qtyInput = row.querySelector(".qty");
                qtyInput.value = (Number(qtyInput.value || 0) + 1).toFixed(2);
                calculateTaxFromRate(row, item.taxRate || 0, item.isTaxable);
                calculateLine(row);
                qtyInput.focus();
                qtyInput.select();
                return true;
            }
        }
        return false;
    }

    function bindLineEvents(row) {
        row.querySelector(".qty").addEventListener("input", () => {
            recalculateTaxIfPossible(row);
            calculateLine(row);
        });

        row.querySelector(".price").addEventListener("input", () => {
            recalculateTaxIfPossible(row);
            calculateLine(row);
        });

        row.querySelector(".discount").addEventListener("input", () => calculateLine(row));
        row.querySelector(".tax").addEventListener("input", () => calculateLine(row));

        row.querySelector(".btn-remove-line").addEventListener("click", () => {
            row.remove();
            calculateTotals();
        });
    }

    function recalculateTaxIfPossible(row) {
        const taxRate = row.dataset.taxRate;
        const isTaxable = row.dataset.isTaxable === "true";
        if (taxRate) {
            calculateTaxFromRate(row, Number(taxRate), isTaxable);
        }
    }

    function calculateTaxFromRate(row, taxRate, isTaxable) {
        row.dataset.taxRate = taxRate;
        row.dataset.isTaxable = isTaxable ? "true" : "false";

        if (!isTaxable || Number(taxRate) <= 0) {
            row.querySelector(".tax").value = "0.00";
            return;
        }

        const qty = Number(row.querySelector(".qty").value || 0);
        const price = Number(row.querySelector(".price").value || 0);
        const discount = Number(row.querySelector(".discount").value || 0);

        const taxableBase = (qty * price) - discount;
        const tax = taxableBase > 0 ? (taxableBase * Number(taxRate)) / 100 : 0;
        row.querySelector(".tax").value = tax.toFixed(2);
    }

    function calculateLine(row) {
        const qty = Number(row.querySelector(".qty").value || 0);
        const price = Number(row.querySelector(".price").value || 0);
        const discount = Number(row.querySelector(".discount").value || 0);
        const tax = Number(row.querySelector(".tax").value || 0);

        const total = (qty * price) - discount + tax;
        row.querySelector(".line-total").value = total.toFixed(2);

        validateStock(row);
        calculateTotals();
    }

    function validateStock(row) {
        const qty = Number(row.querySelector(".qty").value || 0);
        const available = Number(row.querySelector(".available-qty").value || 0);

        if (qty > available) {
            row.classList.add("table-danger");
        } else {
            row.classList.remove("table-danger");
        }
    }

    function calculateTotals() {
        const rows = body.querySelectorAll("tr");

        let subTotal = 0;
        let totalDiscount = 0;
        let totalTax = 0;
        let net = 0;

        rows.forEach(row => {
            const qty = Number(row.querySelector(".qty").value || 0);
            const price = Number(row.querySelector(".price").value || 0);
            const discount = Number(row.querySelector(".discount").value || 0);
            const tax = Number(row.querySelector(".tax").value || 0);

            subTotal += qty * price;
            totalDiscount += discount;
            totalTax += tax;
            net += (qty * price) - discount + tax;
        });

        subTotalInput.value = subTotal.toFixed(2);
        discountInput.value = totalDiscount.toFixed(2);
        taxInput.value = totalTax.toFixed(2);
        netInput.value = net.toFixed(2);
        calculatePaymentTotals();
    }

    async function findItem(q) {
        const warehouseId = warehouseSelect?.value || "";
        if (!warehouseId) {
            alert("اختر المخزن أولًا");
            return null;
        }

        const res = await fetch(`/Pos/FindItem?q=${encodeURIComponent(q)}&warehouseId=${encodeURIComponent(warehouseId)}`);
        const data = await res.json();

        if (!data.ok) {
            alert(data.message || "الصنف غير موجود");
            return null;
        }

        return data.item;
    }

    searchInput.addEventListener("keydown", async function (e) {
        if (e.key === "Escape") {
            searchInput.value = "";
            return;
        }

        if (e.key !== "Enter") return;

        e.preventDefault();
        const q = searchInput.value.trim();
        if (!q) return;

        const item = await findItem(q);
        if (item) {
            addLine(item);
            searchInput.value = "";
            searchInput.focus();
        }
    });

    btnAddLine.addEventListener("click", function () {
        addLine();
    });

    btnClearLines.addEventListener("click", function () {
        body.innerHTML = "";
        paymentsBody.innerHTML = "";
        addPaymentRow();
        calculateTotals();
        searchInput.focus();
    });

    document.addEventListener("keydown", function (e) {
        if (e.key === "F2") {
            e.preventDefault();
            searchInput.focus();
            searchInput.select();
        }
    });
    if (btnAddPayment) {
        btnAddPayment.addEventListener("click", function () {
            addPaymentRow();
        });
    }
    form.addEventListener("submit", function (e) {
        if (e.submitter && e.submitter.id === "btn-save-pos") {
            e.preventDefault();
            return;
        }
        const rows = body.querySelectorAll("tr");

        if (!rows.length) {
            e.preventDefault();
            alert("أضف صنفًا واحدًا على الأقل");
            return;
        }

        const invalidRow = Array.from(rows).find(row => row.classList.contains("table-danger"));
        if (invalidRow) {
            e.preventDefault();
            alert("يوجد صنف كميته أكبر من الرصيد المتاح");
            return;
        }

        document.querySelectorAll(".dynamic-pos-hidden").forEach(x => x.remove());
        if (btnSavePos) {
            btnSavePos.addEventListener("click", function () {
                savePosAjax();
            });
        }
        if (btnReprintLast) {
            btnReprintLast.addEventListener("click", function () {
                if (!lastPrintUrl) {
                    alert("لا توجد فاتورة محفوظة حديثًا لإعادة طباعتها");
                    return;
                }

                window.open(lastPrintUrl, "_blank");
            });
        }
        rows.forEach((row, index) => {
            appendHidden(`Lines[${index}].ItemId`, row.querySelector(".item-id").value);
            appendHidden(`Lines[${index}].ItemCode`, row.querySelector(".item-code").value);
            appendHidden(`Lines[${index}].ItemNameAr`, row.querySelector(".item-name").value);
            appendHidden(`Lines[${index}].Quantity`, row.querySelector(".qty").value);
            appendHidden(`Lines[${index}].UnitPrice`, row.querySelector(".price").value);
            appendHidden(`Lines[${index}].DiscountAmount`, row.querySelector(".discount").value);
            appendHidden(`Lines[${index}].TaxAmount`, row.querySelector(".tax").value);
            appendHidden(`Lines[${index}].LineTotal`, row.querySelector(".line-total").value);
        });

        calculateTotals();
    });

    function appendHidden(name, value) {
        const input = document.createElement("input");
        input.type = "hidden";
        input.name = name;
        input.value = value;
        input.classList.add("dynamic-pos-hidden");
        form.appendChild(input);
    }
    document.querySelectorAll("#pos-payments-body tr").forEach(bindPaymentEvents);
    calculatePaymentTotals();
})();