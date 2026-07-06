import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { apiFetch } from "../services/api";

type UserProfile = {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    profilePictureUrl?: string;
};

function Profile() {
    const [profile, setProfile] = useState<UserProfile | null>(null);
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [message, setMessage] = useState("");
    const [showUpload, setShowUpload] = useState(false);
    const navigate = useNavigate();
    const fileInputRef = useRef<HTMLInputElement | null>(null);

    useEffect(() => {
        async function loadProfile() {
            try {
                const data = await apiFetch("/api/user/profile");
                setProfile(data);
                setFirstName(data.firstName);
                setLastName(data.lastName);
            } catch {
                setMessage("Failed to load profile.");
            }
        }

        loadProfile();
    }, []);

    function handleLogout() {
        localStorage.removeItem("token");
        navigate("/");
    }

    async function handleSave(e: React.SubmitEvent) {
        e.preventDefault();

        setMessage("");

        try {
            const updated = await apiFetch("/api/user/profile", {
                method: "PUT",
                body: JSON.stringify({ firstName, lastName }),
            });

            setProfile(updated);
            setMessage("Profile updated successfully.");
        } catch (err) {
            if (err instanceof Error) {
                setMessage(err.message);
            } else {
                setMessage("An unexpected error occurred.");
            }
        }
    }

    async function uploadPicture(file: File) {
        setMessage("");

        try {
            const formData = new FormData();
            formData.append("file", file);

            const result = await apiFetch("/api/user/profile-picture", {
                method: "POST",
                body: formData,
            });

            setProfile((prev) =>
                prev ? { ...prev, profilePictureUrl: result.profilePictureUrl } : prev
            );

            setMessage("Profile picture updated successfully.");
            setShowUpload(false);
        } catch (err) {
            if (err instanceof Error) {
                setShowUpload(false);
                setMessage(err.message);
            } else {
                setShowUpload(false);
                setMessage("An unexpected error occurred.");
            }
        }
    }

    function handlePictureChange(e: React.ChangeEvent<HTMLInputElement>) {
        const file = e.target.files?.[0];
        if (file) uploadPicture(file);
    }

    function handleDrop(e: React.DragEvent<HTMLDivElement>) {
        e.preventDefault();

        const file = e.dataTransfer.files?.[0];
        if (file) uploadPicture(file);
    }

    function handleDragOver(e: React.DragEvent<HTMLDivElement>) {
        e.preventDefault();
    }


    if (!profile) return <p>Loading...</p>;

    return (
        <div
            style={{
                position: "relative",
                minHeight: "100vh",
            }}
        >
            <button
                type="button"
                onClick={handleLogout}
                style={{
                    position: "absolute",
                    top: "20px",
                    right: "20px",
                }}
            >
                Logout
            </button>
            <div style={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                justifyContent: "center",
                height: "50vh",
            }}>

                <img
                    src={profile.profilePictureUrl || "/default.jpg"}
                    alt="Profile"
                    width="200"
                    height="200"
                />

                <button
                    type="button"
                    onClick={() => setShowUpload(!showUpload)}
                >
                    Change Profile Picture
                </button>

                <div><br /></div>

                {showUpload && (
                    <>
                        <div
                            onClick={() => setShowUpload(false)}
                            style={{
                                position: "fixed",
                                inset: 0,
                                backgroundColor: "rgba(0,0,0,0.5)",
                                zIndex: 999,
                            }}
                        />

                        <div
                            onDrop={handleDrop}
                            onDragOver={handleDragOver}
                            style={{
                                position: "fixed",
                                backgroundColor: "white",
                                padding: "32px",
                                zIndex: 1000,
                            }}
                        >
                            <p>Drag and drop an image here</p>

                            <p>or</p>

                            <button
                                type="button"
                                onClick={() => fileInputRef.current?.click()}
                            >
                                Choose File
                            </button>

                            <br />
                            <br />

                            <button
                                type="button"
                                onClick={() => setShowUpload(false)}
                            >
                                Cancel
                            </button>

                            <input
                                ref={fileInputRef}
                                type="file"
                                accept="image/png,image/jpeg"
                                onChange={handlePictureChange}
                                style={{ display: "none" }}
                            />
                        </div>
                    </>
                )}

                <form
                    style={{
                        display: "grid",
                        gridTemplateColumns: "120px 1fr",
                        gap: "12px",
                        width: "400px",
                    }}
                    onSubmit={handleSave}>


                    <label>First Name</label>
                    <input
                        maxLength={100}
                        value={firstName}
                        onChange={(e) => setFirstName(e.target.value)}
                    />

                    <label>Last Name</label>
                    <input
                        maxLength={100}
                        value={lastName}
                        onChange={(e) => setLastName(e.target.value)}
                    />

                    <label>Email</label>
                    <input
                        value={profile.email}
                        readOnly
                        style={{
                            backgroundColor: "#f5f5f5",
                            border: "1px solid #ccc",
                            color: "#555"
                        }}
                    />

                    <div></div>

                    <p>{message}</p>

                    <div></div>

                    <button style={{ width: "50%" }} type="submit" >
                        Save Changes
                    </button>

                </form>
            </div>
        </div>
    );
}

export default Profile;