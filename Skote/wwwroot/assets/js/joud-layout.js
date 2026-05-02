(function () {
  "use strict";

  var STORAGE_KEY = "joud.sidebar.collapsed";

  function isCollapsed() {
    return document.body.classList.contains("vertical-collpsed");
  }

  /* Use Skote's native class system — same as app.js does */
  function applyState(collapsed) {
    if (collapsed && window.innerWidth >= 992) {
      document.body.classList.add("vertical-collpsed", "sidebar-enable");
    } else {
      document.body.classList.remove("vertical-collpsed", "sidebar-enable");
    }
    updateBrandCopy(collapsed && window.innerWidth >= 992);
  }

  function toggleSidebar() {
    var nextCollapsed = !isCollapsed();

    document.body.classList.toggle("sidebar-enable");
    if (window.innerWidth >= 992) {
      document.body.classList.toggle("vertical-collpsed");
    } else {
      document.body.classList.remove("vertical-collpsed");
    }

    var nowCollapsed = isCollapsed();
    try { localStorage.setItem(STORAGE_KEY, nowCollapsed ? "1" : "0"); } catch (e) {}
    updateBrandCopy(nowCollapsed);
  }

  function updateBrandCopy(hidden) {
    document.querySelectorAll(".joud-brand-copy").forEach(function (el) {
      el.style.opacity = hidden ? "0" : "";
      el.style.maxWidth = hidden ? "0" : "";
      el.style.overflow = hidden ? "hidden" : "";
    });
  }

  /* MetisMenu — init only once */
  function initMetisMenu() {
    if (typeof jQuery === "undefined" || !jQuery.fn.metisMenu) return;
    if (!jQuery("#side-menu").length) return;
    if (jQuery("#side-menu").data("metismenu")) return; /* already done by app.js */
    jQuery("#side-menu").metisMenu();
  }

  /* Active menu highlight */
  function initActiveMenu() {
    if (typeof jQuery === "undefined") return;
    var pageUrl = window.location.href.split(/[?#]/)[0];
    jQuery("#sidebar-menu a").each(function () {
      if (this.href === pageUrl) {
        jQuery(this).addClass("active")
          .closest("li").addClass("mm-active")
          .closest("ul.sub-menu").addClass("mm-show")
          .closest("li").addClass("mm-active");
      }
    });
  }

  /* Scroll active item into view */
  function scrollActiveMenu() {
    if (typeof jQuery === "undefined") return;
    if (!jQuery("#sidebar-menu .mm-active .active").length) return;
    var top = jQuery("#sidebar-menu .mm-active .active").offset().top;
    if (top > 300) {
      jQuery(".vertical-menu .simplebar-content-wrapper")
        .animate({ scrollTop: top - 300 }, "slow");
    }
  }

  /*
   * Override app.js jQuery handler (if loaded on this page).
   * We remove app.js click handler then let our native listener handle it.
   * This prevents double-firing.
   */
  function overrideAppJs() {
    if (typeof jQuery === "undefined") return;
    jQuery("#vertical-menu-btn").off("click");
  }

  /* Native click — delegation so it works regardless of timing */
  document.addEventListener("click", function (e) {
    if (!e.target.closest("#vertical-menu-btn, [data-joud-sidebar-toggle]")) return;
    e.preventDefault();
    e.stopImmediatePropagation();
    toggleSidebar();
  }, true); /* capture phase — fires before jQuery bubbling */

  /* Main init */
  function runInit() {
    var saved = false;
    try { saved = localStorage.getItem(STORAGE_KEY) === "1"; } catch (e) {}
    applyState(saved);

    initMetisMenu();
    initActiveMenu();
    scrollActiveMenu();

    overrideAppJs();
    setTimeout(overrideAppJs, 500);
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", runInit);
  } else {
    runInit();
  }

  window.JoudLayout = { toggleSidebar: toggleSidebar };
})();
