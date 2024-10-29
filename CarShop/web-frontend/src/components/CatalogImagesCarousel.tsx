import {useEffect, useState} from "react";

type Params = {
    imageUrls: string[],
    onSelectedImageChange?: (index: number) => void
}

enum SideButton {
    Left,
    Right
}

export default function CatalogImagesCarousel({imageUrls, onSelectedImageChange}: Params) {
    const [selectedImageIndex, setSelectedImageIndex] = useState<number>(0);
    const [sideButtonsDisabled, setSideButtonsDisabled] = useState<boolean>(false);

    if (imageUrls.length === 0)
        throw new Error('Должна быть минимум 1 картинка.')

    useEffect(() => {
        if (selectedImageIndex >= imageUrls.length) {
            handleSelectedImageChange(imageUrls.length - 1);
        } else if (selectedImageIndex < 0) {
            handleSelectedImageChange(0);
        }
    }, [imageUrls]);

    function handleSelectedImageChange(newSelectedImageIndex: number) {
        setSelectedImageIndex(newSelectedImageIndex);
        if (onSelectedImageChange) {
            onSelectedImageChange(newSelectedImageIndex);
        }
    }

    function handleSideButtonsClick(sideButton: SideButton) {
        let newSelectedImageIndex = selectedImageIndex;
        if (sideButton === SideButton.Left) {
            newSelectedImageIndex--;
            if (newSelectedImageIndex < 0)
                newSelectedImageIndex = imageUrls.length - 1;
        } else if (sideButton === SideButton.Right) {
            newSelectedImageIndex++;
            if (newSelectedImageIndex >= imageUrls.length)
                newSelectedImageIndex = 0;
        }

        handleSelectedImageChange(newSelectedImageIndex);
        setSideButtonsDisabled(true);
        setTimeout(setSideButtonsDisabled, 800, false);
    }

    return (
        <>
            {imageUrls.length > 0 && (
                <div id="carouselExampleIndicators" className="carousel slide" style={{backgroundColor: 'darkgrey'}}>
                    <div className="carousel-indicators">
                        {imageUrls.map((_, index) => {
                            return (
                                <button key={index} type="button" data-bs-target="#carouselExampleIndicators"
                                        data-bs-slide-to={index}
                                        className={`${selectedImageIndex === index ? 'active' : ''}`}
                                        aria-current="true" aria-label={`Slide ${index + 1}`}
                                        onClick={() => handleSelectedImageChange(index)}></button>
                            )
                        })}
                    </div>
                    <div className="carousel-inner">
                        {imageUrls.map((imgUrl, index) => {
                            return (
                                <div key={index} className={`carousel-item ${selectedImageIndex === index ? 'active' : ''}`}>
                                    <img src={imgUrl} style={{height: '400px', maxWidth: '1000px'}}
                                         className="d-block mx-auto"/>
                                </div>
                            )
                        })}
                    </div>
                    <button className="carousel-control-prev" type="button" data-bs-target="#carouselExampleIndicators"
                            data-bs-slide="prev"
                            onClick={() => handleSideButtonsClick(SideButton.Left)}
                            disabled={sideButtonsDisabled}>
                        <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                        <span className="visually-hidden">Previous</span>
                    </button>
                    <button className="carousel-control-next" type="button" data-bs-target="#carouselExampleIndicators"
                            data-bs-slide="next"
                            onClick={() => handleSideButtonsClick(SideButton.Right)}
                            disabled={sideButtonsDisabled}>
                        <span className="carousel-control-next-icon" aria-hidden="true"></span>
                        <span className="visually-hidden">Next</span>
                    </button>
                </div>
            )}
        </>
    )
}