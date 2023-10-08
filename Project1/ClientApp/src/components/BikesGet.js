import React from 'react';
import axios from 'axios';

export default class BikesGet extends React.Component {
    state = {
        Bikes: []
    }

    componentDidMount() {
        axios.get(`BikeDepot`)
            .then(res => {
                const Bikes = res.data;
                this.setState({ Bikes });
            })
    }

    render() {
        return (
            <ul>
                {
                    this.state.Bikes
                        .map(Bikes =>
                            <li key={Bikes.bikeId}>{Bikes.bikeName},{Bikes.bikeDescription}</li>
                        )
                }
            </ul>
        )
    }
}