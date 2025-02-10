async function fetchUser() {
    const userId = document.getElementById("userId").value;

    if (!userId) {
        alert("Please enter a user ID.");
        return;
    }

    try {
        const response = await fetch(`/Admin/GetUserById/${userId}`);

        if (response.ok) {
            const user = await response.json();

            console.log("API Response:", user); // Log the response for debugging

            // Display user details
            document.getElementById("userDetails").innerHTML = `
                <div class="container my-5">
                    <h2 class="text-center mb-4">User Details</h2>

                    <div class="row justify-content-center">
                        <div class="col-md-8">
                            <!-- User Details Card -->
                            <div class="card shadow-sm">
                                <div class="card-body">
                                    <h3 class="card-title">User Details</h3>
                                    <hr />
                                    <p><strong>ID:</strong> ${userId}</p>
                                    <p><strong>Name:</strong> ${user.firstName} ${user.lastName}</p>
                                    <p><strong>Email:</strong> ${user.email}</p>
                                    <p><strong>Phone Number:</strong> ${user.phoneNumber}</p>
                                    <p><strong>Address:</strong> ${user.address}</p>
                                    <p><strong>Date of Birth:</strong> ${new Date(user.dateOfBirth).toLocaleDateString()}</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            `;
        } else if (response.status === 404) {
            // Handle user not found
            document.getElementById("userDetails").innerHTML = `<p style="color: red;">User not found.</p>`;
        } else {
            // Handle other errors
            const errorMessage = await response.text(); // API may return a plain-text error
            document.getElementById("userDetails").innerHTML = `<p style="color: red;">Error: ${errorMessage}</p>`;
        }
    } catch (error) {
        console.error("Fetch error:", error);
        document.getElementById("userDetails").innerHTML = `<p style="color: red;">Failed to fetch user details. Please try again later.</p>`;
    }
}
