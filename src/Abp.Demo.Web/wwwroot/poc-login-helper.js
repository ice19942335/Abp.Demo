(() => {
  const run = () => {
    const picker = document.getElementById("demo-user-picker");
    if (!picker) {
      return;
    }

    const userInput = document.querySelector('[data-test-id="login-username"]');
    const passwordInput = document.querySelector('[data-test-id="login-password"]');
    const roleHint = document.getElementById("demo-user-role-hint");

    const applySelection = () => {
      const selectedOption = picker.options[picker.selectedIndex];
      if (!selectedOption || !selectedOption.value) {
        if (roleHint) {
          roleHint.textContent = "";
        }
        return;
      }

      if (userInput) {
        userInput.value = selectedOption.value;
      }

      if (passwordInput) {
        passwordInput.value = selectedOption.dataset.password || "";
      }

      if (roleHint) {
        const roleName = selectedOption.dataset.role || "";
        roleHint.textContent = roleName ? `Role: ${roleName}` : "";
      }
    };

    picker.addEventListener("change", applySelection);

    if (picker.options.length > 1) {
      picker.selectedIndex = 1;
      applySelection();
    }
  };

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", run);
  } else {
    run();
  }
})();
